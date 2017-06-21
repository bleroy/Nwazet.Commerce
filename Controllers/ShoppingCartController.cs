using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.MediaLibrary.Fields;
using Orchard.Mvc;
using Orchard.Themes;
using Orchard.UI.Notify;
using Orchard.Workflows.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

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
        private readonly INotifier _notifier;
        private readonly IEnumerable<IProductAttributeExtensionProvider> _attributeExtensionProviders;
        private readonly ICurrencyProvider _currencyProvider;
        private readonly ILocalStorageSettings _localStorageSettings;
        private readonly IEnumerable<ICartLifeCycleEventHandler> _cartLifeCycleEventHandlers;

        public Localizer T { get; set; }

        private const string AttributePrefix = "productattributes.a";
        private const string ExtensionPrefix = "ext.";

        public ShoppingCartController(
            IShoppingCart shoppingCart,
            IShapeFactory shapeFactory,
            IContentManager contentManager,
            IWorkContextAccessor wca,
            IEnumerable<ICheckoutService> checkoutServices,
            IEnumerable<IShippingMethodProvider> shippingMethodProviders,
            IEnumerable<IExtraCartInfoProvider> extraCartInfoProviders,
            IWorkflowManager workflowManager,
            INotifier notifier,
            IEnumerable<IProductAttributeExtensionProvider> attributeExtensionProviders,
            ICurrencyProvider currencyProvider,
            ILocalStorageSettings localStorageSettings,
            IEnumerable<ICartLifeCycleEventHandler> cartLifeCycleEventHandlers) {

            _shippingMethodProviders = shippingMethodProviders;
            _shoppingCart = shoppingCart;
            _shapeFactory = shapeFactory;
            _contentManager = contentManager;
            _wca = wca;
            _checkoutServices = checkoutServices;
            _extraCartInfoProviders = extraCartInfoProviders;
            _workflowManager = workflowManager;
            _notifier = notifier;
            _attributeExtensionProviders = attributeExtensionProviders;
            _currencyProvider = currencyProvider;
            _localStorageSettings = localStorageSettings;
            _cartLifeCycleEventHandlers = cartLifeCycleEventHandlers;

            T = NullLocalizer.Instance;
        }

        [HttpPost]
        public ActionResult Add(int id, int quantity = 1, bool isAjaxRequest = false) {
            // Manually parse product attributes because of a breaking change
            // in MVC 5 dictionary model binding
            var form = HttpContext.Request.Form;
            var files = HttpContext.Request.Files;
            var productattributes = form.AllKeys
                .Where(key => key.StartsWith(AttributePrefix))
                .ToDictionary(
                    key => int.Parse(key.Substring(AttributePrefix.Length)),
                    key => {
                        var extensionProvider = _attributeExtensionProviders.SingleOrDefault(e => e.Name == form[ExtensionPrefix + key + ".provider"]);
                        Dictionary<string, string> extensionFormValues = null;
                        if (extensionProvider != null) {
                            extensionFormValues = form.AllKeys.Where(k => k.StartsWith(ExtensionPrefix + key + "."))
                                .ToDictionary(
                                    k => k.Substring((ExtensionPrefix + key + ".").Length),
                                    k => form[k]);
                            return new ProductAttributeValueExtended {
                                Value = form[key],
                                ExtendedValue = extensionProvider.Serialize(form[ExtensionPrefix + key], extensionFormValues, files),
                                ExtensionProvider = extensionProvider.Name
                            };
                        }
                        return new ProductAttributeValueExtended {
                            Value = form[key],
                            ExtendedValue = null,
                            ExtensionProvider = null
                        };
                    });

            // Retrieve minimum order quantity
            Dictionary<int, List<string>> productMessages = new Dictionary<int, List<string>>();
            var productPart = _contentManager.Get<ProductPart>(id);
            string productTitle = _contentManager.GetItemMetadata(productPart.ContentItem).DisplayText;
            if (productPart != null) {
                if (quantity < productPart.MinimumOrderQuantity) {
                    quantity = productPart.MinimumOrderQuantity;
                    if (productMessages.ContainsKey(id)) {
                        productMessages[id].Add(T("Quantity increased to match minimum possible for {0}.", productTitle).Text);
                    } else {
                        productMessages.Add(id, new List<string>() { T("Quantity increased to match minimum possible for {0}.", productTitle).Text });
                    }
                }
                //only add to cart if there are at least as many available products as the requested quantity
                if (quantity > productPart.Inventory && !productPart.AllowBackOrder &&
                    (!productPart.IsDigital || (productPart.IsDigital && productPart.ConsiderInventory))
                    ) {
                    quantity = productPart.Inventory;
                    if (productMessages.ContainsKey(id)) {
                        productMessages[id].Add(T("Quantity decreased to match inventory for {0}.", productTitle).Text);
                    } else {
                        productMessages.Add(id, new List<string>() { T("Quantity decreased to match inventory for {0}.", productTitle).Text });
                    }
                }
            }

            _shoppingCart.Add(id, quantity, productattributes);

            var newItem = new ShoppingCartItem(id, quantity, productattributes);
            foreach (var handler in _cartLifeCycleEventHandlers) {
                handler.ItemAdded(newItem);
            }

            // Test isAjaxRequest too because iframe posts won't return true for Request.IsAjaxRequest()
            if (Request.IsAjaxRequest() || isAjaxRequest) {
                return new ShapePartialResult(
                    this,
                    BuildCartShape(true, _shoppingCart.Country, _shoppingCart.ZipCode, null, productMessages));
            }
            return RedirectToAction("Index", productMessages);
        }

        [Themed]
        [OutputCache(Duration = 0)]
        public ActionResult Index(Dictionary<int, List<string>> productMessages = null) {
            _wca.GetContext().Layout.IsCartPage = true;
            try {
                return new ShapeResult(
                    this,
                    BuildCartShape(
                        false,
                        _shoppingCart.Country,
                        _shoppingCart.ZipCode,
                        _shoppingCart.ShippingOption,
                        productMessages));
            } catch (ShippingException ex) {
                _shoppingCart.Country = null;
                _shoppingCart.ZipCode = null;
                _shoppingCart.ShippingOption = null;
                _notifier.Error(new LocalizedString(ex.Message));
                return RedirectToAction("Index");
            }
        }

        private dynamic BuildCartShape(
            bool isSummary = false,
            string country = null,
            string zipCode = null,
            ShippingOption shippingOption = null,
            Dictionary<int, List<string>> productMessages = null) {

            var shape = _shapeFactory.ShoppingCart();

            if (shippingOption != null) {
                shape.ReadOnly = true;
                shape.ShippingOption = shippingOption;
            }

            var productQuantities = _shoppingCart
                .GetProducts()
                .Where(p => p.Quantity > 0)
                .ToList();
            var productShapes = GetProductShapesFromQuantities(productQuantities, productMessages);
            shape.ShopItems = productShapes;

            var shopItemsAllDigital = !(productQuantities.Any(pq => !(pq.Product.IsDigital)));
            shape.ShopItemsAllDigital = shopItemsAllDigital;

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

                if (!shopItemsAllDigital) {
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
            }

            var subtotal = _shoppingCart.Subtotal();
            var taxes = _shoppingCart.Taxes(subtotal);

            // Check to see if any of the products require the user to be authenticated
            var shopItemsAuthenticationRequired = productQuantities.Any(pq => pq.Product.AuthenticationRequired);
            shape.ShopItemsAuthenticationRequired = shopItemsAuthenticationRequired;

            var displayCheckoutButtons = productQuantities.Any();
            var currentUser = _wca.GetContext().CurrentUser;
            if (currentUser == null) {
                if (shopItemsAuthenticationRequired) {
                    displayCheckoutButtons = false;
                }
            }
            if (displayCheckoutButtons) {
                //check whether back-order is allowed for products whose inventory is less than the requested quantity
                displayCheckoutButtons = !productQuantities.Any(pq =>
                    pq.Quantity > pq.Product.Inventory && !pq.Product.AllowBackOrder &&
                    (!pq.Product.IsDigital || (pq.Product.IsDigital && pq.Product.ConsiderInventory)));
            }
            if (displayCheckoutButtons) {
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



            shape.Subtotal = subtotal;
            shape.Taxes = taxes;
            shape.Total = _shoppingCart.Total(subtotal, taxes);
            shape.CurrencyProvider = _currencyProvider;
            if (isSummary) {
                shape.Metadata.Alternates.Add("ShoppingCart_Summary");
            }
            return shape;
        }

        private IEnumerable<dynamic> GetProductShapesFromQuantities(
            IEnumerable<ShoppingCartQuantityProduct> productQuantities,
            Dictionary<int, List<string>> productMessages = null) {
            var productShapes = productQuantities.Select(
                productQuantity => _shapeFactory.ShoppingCartItem(
                    Quantity: productQuantity.Quantity,
                    Product: productQuantity.Product,
                    Sku: productQuantity.Product.Sku,
                    Title: _contentManager.GetItemMetadata(productQuantity.Product).DisplayText,
                    ProductAttributes: productQuantity.AttributeIdsToValues,
                    ContentItem: (productQuantity.Product).ContentItem,
                    ProductImage: ((MediaLibraryPickerField)productQuantity.Product.ContentItem.Parts.SelectMany(part => part.Fields).FirstOrDefault(field => field.Name == "ProductImage")),
                    IsDigital: productQuantity.Product.IsDigital,
                    ConsiderInventory: productQuantity.Product.ConsiderInventory,
                    Price: productQuantity.Product.Price,
                    OriginalPrice: productQuantity.Product.Price,
                    DiscountedPrice: productQuantity.Price,
                    LinePriceAdjustment: productQuantity.LinePriceAdjustment,
                    Promotion: productQuantity.Promotion,
                    ShippingCost: productQuantity.Product.ShippingCost,
                    Weight: productQuantity.Product.Weight,
                    MinimumOrderQuantity: productQuantity.Product.MinimumOrderQuantity,
                    Messages: productMessages == null ?
                        (string)null :
                        productMessages.ContainsKey(productQuantity.Product.Id) ?
                            string.Join(Environment.NewLine, productMessages[productQuantity.Product.Id]) :
                            (string)null,
                    Inventory: productQuantity.Product.Inventory,
                    AllowBackOrder: productQuantity.Product.AllowBackOrder
                    )).ToList();
            return productShapes;
        }

        [OutputCache(Duration = 0)]
        public ActionResult NakedCart() {
            try {
                return new ShapePartialResult(this,
                    BuildCartShape(
                        true,
                        _shoppingCart.Country,
                        _shoppingCart.ZipCode));
            } catch (ShippingException ex) {
                _shoppingCart.Country = null;
                _shoppingCart.ZipCode = null;
                _shoppingCart.ShippingOption = null;
                _notifier.Error(new LocalizedString(ex.Message));
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public JsonResult UseLocalStorage() {
            bool response = _localStorageSettings.UseLocalStorage();
            return (JsonResult)Json(new { Response = response });
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

            UpdateShoppingCart(items == null ? null : items.Reverse());
            try {
                return new ShapePartialResult(this,
                    BuildCartShape(
                        true,
                        _shoppingCart.Country,
                        _shoppingCart.ZipCode));
            } catch (ShippingException ex) {
                _shoppingCart.Country = null;
                _shoppingCart.ZipCode = null;
                _shoppingCart.ShippingOption = null;
                _notifier.Error(new LocalizedString(ex.Message));
                return RedirectToAction("Index");
            }
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
            var oldItems = new List<ShoppingCartItem>();
            oldItems.AddRange(_shoppingCart.Items); //create a copy for analysis
            _shoppingCart.Clear();

            if (items == null) {
                //removed all items
                foreach (var handler in _cartLifeCycleEventHandlers) {
                    //we raise the ItemRemoved event for all the removed items.
                    //The ShoppingCartItem objects also contain the quantity of the variation
                    handler.Updated(new List<ShoppingCartItem>(0), oldItems);
                }
                return;
            }



            var minimumOrderQuantities = GetMinimumOrderQuantities(items);

            _shoppingCart.AddRange(
                items
                    .Where(item => !item.IsRemoved)
                    .Select(item => new ShoppingCartItem(
                                        item.ProductId,
                                        item.Quantity <= 0 ? 0 : item.Quantity < minimumOrderQuantities[item.ProductId] ? minimumOrderQuantities[item.ProductId] : item.Quantity,
                                        item.AttributeIdsToValues))
                );

            _shoppingCart.UpdateItems();
            //analyze update to raise events
            var addedItems = new List<ShoppingCartItem>();
            var removedItems = new List<ShoppingCartItem>();
            var newItems = new List<ShoppingCartItem>();
            newItems.AddRange(_shoppingCart.Items);
            //we use a KeyValuePair because item.QUantity is not alowed to be negative
            var itemsInBoth = new List<KeyValuePair<int,ShoppingCartItem>>(); //quantity variation, item
            itemsInBoth.AddRange(
                newItems
                .Select(newSci => {
                    var oldSci = oldItems
                        .FirstOrDefault(sci => ShoppingCartItem.ItemsAreEqual(sci, newSci)); //item was in cart already
                    int quantityVariation = 0;
                    if (oldSci != null) {
                        quantityVariation = newSci.Quantity - oldSci.Quantity;
                        oldSci.Quantity = Math.Abs(quantityVariation);
                    }
                    return new KeyValuePair<int, ShoppingCartItem>(quantityVariation, oldSci);
                })
                .Where(kvp => kvp.Value != null));
            foreach (var item in itemsInBoth.Where(kvp => kvp.Key != 0)) {
                if (item.Key > 0) { //increase in quantity
                    addedItems.Add(item.Value);
                } else if (item.Key < 0) { //decrease in quantity
                    removedItems.Add(item.Value);
                }
                //if quantity has not changed, we do not raise an event for that item
            }
            addedItems.AddRange(
                newItems.Where(sci => !itemsInBoth.Any(isci => ShoppingCartItem.ItemsAreEqual(isci.Value, sci)))
                );
            removedItems.AddRange(
                oldItems.Where(sci => !itemsInBoth.Any(isci => ShoppingCartItem.ItemsAreEqual(isci.Value, sci)))
                );

            foreach (var handler in _cartLifeCycleEventHandlers) {
                handler.Updated(addedItems, removedItems);
            }
        }

        private Dictionary<int, int> GetMinimumOrderQuantities(IEnumerable<UpdateShoppingCartItemViewModel> items) {
            Dictionary<int, int> minimumOrderQuantites = new Dictionary<int, int>();
            int defaultMinimumQuantity = 1;

            if (items != null) {
                var productIds = items.Select(i => i.ProductId).Distinct();
                var products = _contentManager.GetMany<ProductPart>(productIds, VersionOptions.Published, QueryHints.Empty).ToList();

                // Because a product might not exist (may be unpublished or deleted but still in shopping cart) need to iterate instead of using .ToDictionary
                foreach (var item in items) {
                    // Check to ensure productId doesn't exist (can happen when product in cart multiple times with different attributes
                    if (!minimumOrderQuantites.ContainsKey(item.ProductId)) {
                        var product = products.Where(p => p.Id == item.ProductId).FirstOrDefault();
                        if (product != null) {
                            minimumOrderQuantites.Add(product.Id, product.MinimumOrderQuantity);
                        } else {
                            // This ensures the dictionary will have all the keys needed for the items
                            minimumOrderQuantites.Add(item.ProductId, defaultMinimumQuantity);
                        }
                    }
                }
            }

            return minimumOrderQuantites;
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