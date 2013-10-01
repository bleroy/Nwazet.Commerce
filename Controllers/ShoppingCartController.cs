using System;
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
using Orchard.MediaLibrary.Fields;
using Orchard.Mvc;
using Orchard.Themes;
using Orchard.Workflows.Services;

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
        private readonly IWorkflowManager _workflowManager;

        public ShoppingCartController(
            IShoppingCart shoppingCart,
            IShapeFactory shapeFactory,
            IContentManager contentManager,
            IWorkContextAccessor wca,
            IEnumerable<ICheckoutService> checkoutServices,
            IEnumerable<IShippingMethodProvider> shippingMethodProviders,
            IEnumerable<IExtraCartInfoProvider> extraCartInfoProviders,
            IWorkflowManager workflowManager) {

            _shippingMethodProviders = shippingMethodProviders;
            _shoppingCart = shoppingCart;
            _shapeFactory = shapeFactory;
            _contentManager = contentManager;
            _wca = wca;
            _checkoutServices = checkoutServices;
            _extraCartInfoProviders = extraCartInfoProviders;
            _workflowManager = workflowManager;
        }

        [HttpPost]
        public ActionResult Add(int id, int quantity, IDictionary<int, string> productattributes) {
            // Workaround MVC buggy behavior that won't correctly bind an empty dictionary
            if (productattributes.Count == 1 && productattributes.Values.First() == "__none__") {
                productattributes = null;
            }
            _shoppingCart.Add(id, quantity, productattributes);

            _workflowManager.TriggerEvent("CartUpdated",
                _wca.GetContext().CurrentSite,
                () => new Dictionary<string, object> {
                    {"Cart", _shoppingCart}
                });

            if (Request.IsAjaxRequest()) {
                return new ShapePartialResult(
                    this,
                    BuildCartShape(true, _shoppingCart.Country, _shoppingCart.ZipCode));
            }
            return RedirectToAction("Index");
        }

        [Themed]
        [OutputCache(Duration = 0)]
        public ActionResult Index() {
            _wca.GetContext().Layout.IsCartPage = true;
            return new ShapeResult(
                this,
                BuildCartShape(
                    false,
                    _shoppingCart.Country,
                    _shoppingCart.ZipCode,
                    _shoppingCart.ShippingOption));
        }

        private dynamic BuildCartShape(
            bool isSummary = false,
            string country = null, 
            string zipCode = null, 
            ShippingOption shippingOption = null) {

            var shape = _shapeFactory.ShoppingCart();

            if (shippingOption != null) {
                shape.ReadOnly = true;
                shape.ShippingOption = shippingOption;
            }

            var productQuantities = _shoppingCart.GetProducts().ToList();
            var productShapes = GetProductShapesFromQuantities(productQuantities);
            shape.ShopItems = productShapes;

            var custom =
                _extraCartInfoProviders == null
                    ? null
                    : _extraCartInfoProviders
                          .SelectMany(p => p.GetExtraCartInfo())
                          .ToList();

            if ((country == Country.UnitedStates && !string.IsNullOrWhiteSpace(zipCode)) ||
                (!String.IsNullOrWhiteSpace(country) && country != Country.UnitedStates)) {
                shape.Country = country;
                shape.ZipCode = zipCode;
                if (!isSummary && shippingOption == null) {
                    var shippingMethods = _shippingMethodProviders
                        .SelectMany(p => p.GetShippingMethods())
                        .ToList();
                    shape.ShippingOptions =
                        ShippingService
                            .GetShippingOptions(
                                shippingMethods, productQuantities, country, zipCode, _wca).ToList();
                }
            }
            var taxes = _shoppingCart.Taxes();
            if (shippingOption != null) {
                var checkoutShapes = _checkoutServices.Select(
                    service => service.BuildCheckoutButtonShape(
                        productShapes,
                        productQuantities,
                        new[] { shippingOption },
                        taxes,
                        country,
                        zipCode,
                        custom)).ToList();
                shape.CheckoutButtons = checkoutShapes;
            }

            shape.Subtotal = _shoppingCart.Subtotal();
            shape.Taxes = taxes;
            shape.Total = _shoppingCart.Total();
            if (isSummary)
            {
                shape.Metadata.Alternates.Add("ShoppingCart_Summary");
            }
            return shape;
        }

        private IEnumerable<dynamic> GetProductShapesFromQuantities(
            IEnumerable<ShoppingCartQuantityProduct> productQuantities) {
            var productShapes = productQuantities.Select(
                productQuantity => _shapeFactory.ShoppingCartItem(
                    Quantity: productQuantity.Quantity,
                    Product: productQuantity.Product,
                    Sku: productQuantity.Product.Sku,
                    Title: _contentManager.GetItemMetadata(productQuantity.Product).DisplayText,
                    ProductAttributes: productQuantity.AttributeIdsToValues,
                    ContentItem: (productQuantity.Product).ContentItem,
                    ProductImage:
                        ((MediaLibraryPickerField) productQuantity.Product.Fields.FirstOrDefault(f => f.Name == "ProductImage")),
                    IsDigital: productQuantity.Product.IsDigital,
                    Price: productQuantity.Product.Price,
                    DiscountedPrice: productQuantity.Price,
                    ShippingCost: productQuantity.Product.ShippingCost,
                    Weight: productQuantity.Product.Weight)).ToList();
            return productShapes;
        }

        [OutputCache(Duration = 0)]
        public ActionResult NakedCart() {
            return new ShapePartialResult(this,
                                          BuildCartShape(true, _shoppingCart.Country, _shoppingCart.ZipCode));
        }

        [HttpPost]
        public ActionResult Update(
            UpdateShoppingCartItemViewModel[] items,
            string country = null,
            string zipCode = null,
            string shippingOption = null) {

            _shoppingCart.Country = country;
            _shoppingCart.ZipCode = zipCode;
            _shoppingCart.ShippingOption = String.IsNullOrWhiteSpace(shippingOption) ? null : ShippingService.RebuildShippingOption(shippingOption);

            if (items != null) {
                UpdateShoppingCart(items.Reverse());
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult AjaxUpdate(
            UpdateShoppingCartItemViewModel[] items,
            string country = null,
            string zipCode = null) {

            _shoppingCart.Country = country;
            _shoppingCart.ZipCode = zipCode;
            _shoppingCart.ShippingOption = null;

            UpdateShoppingCart(items.Reverse());
            return new ShapePartialResult(this,
                                          BuildCartShape(true, _shoppingCart.Country, _shoppingCart.ZipCode));
        }

        [OutputCache(Duration = 0)]
        public ActionResult GetItems() {
            var products = _shoppingCart.GetProducts();

            var json = new {
                items = (from productQuantity in products
                         select new {
                             id = productQuantity.Product.Id,
                             title = productQuantity.Product != null
                                         ? _contentManager.GetItemMetadata(productQuantity.Product).DisplayText
                                         : productQuantity.Product.Sku,
                             productAttributes = productQuantity.AttributeIdsToValues,
                             unitPrice = productQuantity.Product.Price,
                             quantity = productQuantity.Quantity
                         }).ToArray()
            };

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        private void UpdateShoppingCart(IEnumerable<UpdateShoppingCartItemViewModel> items) {
            _shoppingCart.Clear();

            if (items == null)
                return;

            _shoppingCart.AddRange(
                items
                    .Where(item => !item.IsRemoved)
                    .Select(item => new ShoppingCartItem(
                                        item.ProductId,
                                        item.Quantity < 0 ? 0 : item.Quantity,
                                        item.AttributeIdsToValues))
                );

            _shoppingCart.UpdateItems();

            _workflowManager.TriggerEvent("CartUpdated",
                _wca.GetContext().CurrentSite,
                () => new Dictionary<string, object> {
                    {"Cart", _shoppingCart}
                });
        }

        public ActionResult ResetDestination() {
            _shoppingCart.Country = null;
            _shoppingCart.ZipCode = null;

            return RedirectToAction("Index");
        }

        public ActionResult ResetShippingOption() {
            _shoppingCart.ShippingOption = null;

            return RedirectToAction("Index");
        }
    }
}