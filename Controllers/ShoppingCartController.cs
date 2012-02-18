using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard.Mvc;
using Orchard.Themes;

namespace Nwazet.Commerce.Controllers {
    [OrchardFeature("Nwazet.Commerce")]
    public class ShoppingCartController : Controller {
        private readonly IShoppingCart _shoppingCart;
        private readonly dynamic _shapeFactory;
        private readonly IContentManager _contentManager;
        private readonly IWorkContextAccessor _wca;
        private readonly IEnumerable<ICheckoutService> _checkoutServices;
        private readonly IEnumerable<IShippingMethodProvider> _shippingMethodProviders;
        private readonly IEnumerable<IExtraCartInfoProvider> _extraCartInfoProviders; 

        public ShoppingCartController(
            IShoppingCart shoppingCart,
            IShapeFactory shapeFactory,
            IContentManager contentManager,
            IWorkContextAccessor wca,
            IEnumerable<ICheckoutService> checkoutServices,
            IEnumerable<IShippingMethodProvider> shippingMethodProviders,
            IEnumerable<IExtraCartInfoProvider> extraCartInfoProviders) {

            _shippingMethodProviders = shippingMethodProviders;
            _shoppingCart = shoppingCart;
            _shapeFactory = shapeFactory;
            _contentManager = contentManager;
            _wca = wca;
            _checkoutServices = checkoutServices;
            _extraCartInfoProviders = extraCartInfoProviders;
        }

        [HttpPost]
        public ActionResult Add(int id, int quantity) {
            _shoppingCart.Add(id, quantity);
            if (Request.IsAjaxRequest()) {
                return new ShapePartialResult(this, BuildCartShape(true));
            }
            return RedirectToAction("Index");
        }

        [Themed]
        [OutputCache(Duration = 0)]
        public ActionResult Index() {
            _wca.GetContext().Layout.IsCartPage = true;
            return new ShapeResult(this, BuildCartShape());
        }

        private dynamic BuildCartShape(bool isSummary = false) {
            dynamic shape = _shapeFactory.ShoppingCart();

            var productQuantities = _shoppingCart.GetProducts();
            var productShapes = productQuantities.Select(
                productQuantity => _shapeFactory.ShoppingCartItem(
                    Quantity: productQuantity.Quantity,
                    Product: productQuantity.Product,
                    Sku: productQuantity.Product.Sku,
                    Title: productQuantity.Product is IContent ? _contentManager.GetItemMetadata((IContent)productQuantity.Product).DisplayText : productQuantity.Product.Sku,
                    ContentItem: (productQuantity.Product is IContent ? ((IContent)productQuantity.Product).ContentItem : null),
                    ProductImage: ((dynamic)productQuantity.Product).Product.ProductImage,
                    IsDigital: productQuantity.Product.IsDigital,
                    Price: productQuantity.Product.Price,
                    ShippingCost: productQuantity.Product.ShippingCost,
                    Weight: productQuantity.Product.Weight)).ToList();
            shape.ShopItems = productShapes;

            var validShippingMethods = _shippingMethodProviders
                .SelectMany(p => p.GetShippingMethods())
                .Select(
                    m => _shapeFactory.ShippingMethod(
                        Price: m.ComputePrice(productQuantities),
                        DisplayName: _contentManager.GetItemMetadata(m).DisplayText,
                        Name: m.Name,
                        ShippingCompany: m.ShippingCompany,
                        IncludedShippingAreas: m.IncludedShippingAreas == null ? null : m.IncludedShippingAreas.Split(','),
                        ExcludedShippingAreas: m.ExcludedShippingAreas == null ? null : m.ExcludedShippingAreas.Split(',')
                             ))
                .Where(x => x.Price >= 0)
                .ToList();

            var custom = _extraCartInfoProviders == null ? null :
                _extraCartInfoProviders
                    .SelectMany(p => p.GetExtraCartInfo())
                    .ToList();

            var checkoutShapes = _checkoutServices.Select(
                service => service.BuildCheckoutButtonShape(
                    productShapes, productQuantities, validShippingMethods, custom)
                );
            shape.CheckoutButtons = checkoutShapes;

            shape.Total = _shoppingCart.Total();
            shape.Subtotal = _shoppingCart.Subtotal();
            shape.Vat = _shoppingCart.Taxes();
            if (isSummary) {
                shape.Metadata.Alternates.Add("ShoppingCart_Summary");
            }
            return shape;
        }

        [OutputCache(Duration = 0)]
        public ActionResult NakedCart() {
            return new ShapePartialResult(this, BuildCartShape(true));
        }

        [HttpPost]
        public ActionResult Update(string command, UpdateShoppingCartItemViewModel[] items){

            UpdateShoppingCart(items);

            switch(command) {
                //case "Checkout":
                //    return RedirectToAction("SignupOrLogin", "Checkout");
                case "Update":
                    break;
            }
            if (Request.IsAjaxRequest()) {
                return new ShapePartialResult(this, BuildCartShape(true));
            }
            return RedirectToAction("Index");
        }

        [OutputCache(Duration = 0)]
        public ActionResult GetItems() {
            var products = _shoppingCart.GetProducts();

            var json = new
            {
                items = (from productQuantity in products
                         select new
                         {
                             id = productQuantity.Product.Id,
                             title = productQuantity.Product is IContent ? _contentManager.GetItemMetadata((IContent)productQuantity.Product).DisplayText : productQuantity.Product.Sku,
                             unitPrice = productQuantity.Product.Price,
                             quantity = productQuantity.Quantity
                         }).ToArray()
            };

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        private void UpdateShoppingCart(IEnumerable<UpdateShoppingCartItemViewModel> items)
        {
            _shoppingCart.Clear();

            if (items == null)
                return;

            _shoppingCart.AddRange(items
                .Where(item => !item.IsRemoved)
                .Select(item => new ShoppingCartItem(item.ProductId, item.Quantity < 0 ? 0 : item.Quantity))
            );

            _shoppingCart.UpdateItems();
        }
    }
}