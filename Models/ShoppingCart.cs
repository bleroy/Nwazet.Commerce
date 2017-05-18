using System;
using System.Collections.Generic;
using System.Linq;
using Nwazet.Commerce.Services;
using Orchard.Autoroute.Models;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.UI.Notify;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Commerce")]
    public class ShoppingCart : IShoppingCart {
        private readonly IContentManager _contentManager;
        private readonly IShoppingCartStorage _cartStorage;
        private readonly IPriceService _priceService;
        private readonly IEnumerable<IProductAttributesDriver> _attributesDrivers;
        private readonly IEnumerable<ITaxProvider> _taxProviders;
        private readonly INotifier _notifier;

        private IEnumerable<ShoppingCartQuantityProduct> _products; 

        public ShoppingCart(
            IContentManager contentManager,
            IShoppingCartStorage cartStorage,
            IPriceService priceService,
            IEnumerable<IProductAttributesDriver> attributesDrivers,
            IEnumerable<ITaxProvider> taxProviders,
            INotifier notifier) {

            _contentManager = contentManager;
            _cartStorage = cartStorage;
            _priceService = priceService;
            _attributesDrivers = attributesDrivers;
            _taxProviders = taxProviders;
            _notifier = notifier;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public IEnumerable<ShoppingCartItem> Items {
            get { return ItemsInternal.AsReadOnly(); }
        }

        public string Country {
            get { return _cartStorage.Country; }
            set { _cartStorage.Country = value; }
        }

        public string ZipCode {
            get { return _cartStorage.ZipCode; }
            set { _cartStorage.ZipCode = value; }
        }

        public ShippingOption ShippingOption {
            get { return _cartStorage.ShippingOption; }
            set { _cartStorage.ShippingOption = value; }
        }

        private List<ShoppingCartItem> ItemsInternal {
            get {
                return _cartStorage.Retrieve();
            }
        }

        public void Add(int productId, int quantity = 1, IDictionary<int, ProductAttributeValueExtended> attributeIdsToValues = null) {
            if (!ValidateAttributes(productId, attributeIdsToValues)) {
                // If attributes don't validate, don't add the product, but notify
                _notifier.Warning(T("Couldn't add this product because of invalid attributes. Please refresh the page and try again."));
                return;
            }
            var item = FindCartItem(productId, attributeIdsToValues);
            if (item != null) {
                item.Quantity += quantity;
            }
            else {
                ItemsInternal.Insert(0, new ShoppingCartItem(productId, quantity, attributeIdsToValues));
            }
            _products = null;
        }

        public ShoppingCartItem FindCartItem(int productId, IDictionary<int, ProductAttributeValueExtended> attributeIdsToValues = null) {
            if (attributeIdsToValues == null || attributeIdsToValues.Count == 0) {
                return Items.FirstOrDefault(i => i.ProductId == productId
                      && (i.AttributeIdsToValues == null || i.AttributeIdsToValues.Count == 0));
            }
            return Items.FirstOrDefault(
                i => i.ProductId == productId
                     && i.AttributeIdsToValues != null
                     && i.AttributeIdsToValues.Count == attributeIdsToValues.Count
                     && i.AttributeIdsToValues.All(attributeIdsToValues.Contains));
        }

        private bool ValidateAttributes(int productId, IDictionary<int, ProductAttributeValueExtended> attributeIdsToValues) {
            if (_attributesDrivers == null ||
                attributeIdsToValues == null ||
                !attributeIdsToValues.Any()) return true;

            var product = _contentManager.Get(productId);
            return _attributesDrivers.All(d => d.ValidateAttributes(product, attributeIdsToValues));
        }

        public void AddRange(IEnumerable<ShoppingCartItem> items) {
            foreach (var item in items) {
                Add(item.ProductId, item.Quantity, item.AttributeIdsToValues);
            }
        }

        public void Remove(int productId, IDictionary<int, ProductAttributeValueExtended> attributeIdsToValues = null) {
            var item = FindCartItem(productId, attributeIdsToValues);
            if (item == null) return;

            ItemsInternal.Remove(item);
            _products = null;
        }

        public IEnumerable<ShoppingCartQuantityProduct> GetProducts() {
            if (_products != null) return _products;

            var ids = Items.Select(x => x.ProductId);

            var productParts =
                _contentManager.GetMany<ProductPart>(ids, VersionOptions.Published,
                new QueryHints().ExpandParts<TitlePart, ProductPart, AutoroutePart>()).ToArray();

            var productPartIds = productParts.Select(p => p.Id);

            var shoppingCartQuantities =
                (from item in Items  
                 where productPartIds.Contains(item.ProductId) && item.Quantity > 0
                 select new ShoppingCartQuantityProduct(item.Quantity, productParts.First(p => p.Id == item.ProductId), item.AttributeIdsToValues))
                    .ToList();

            return _products = shoppingCartQuantities
                .Select(q => _priceService.GetDiscountedPrice(q, shoppingCartQuantities))
                .Where(q => q.Quantity > 0)
                .ToList();
        }

        public void UpdateItems() {
            ItemsInternal.RemoveAll(x => x.Quantity <= 0);
            _products = null;
        }

        public decimal Subtotal() {
            return Math.Round(GetProducts().Sum(pq => Math.Round(pq.Price * pq.Quantity + pq.LinePriceAdjustment, 2)), 2);
        }

        public TaxAmount Taxes(decimal subTotal = 0) {
            if (Country == null && ZipCode == null) return null;
            var taxes = _taxProviders
                .SelectMany(p => p.GetTaxes())
                .OrderByDescending(t => t.Priority);
            var shippingPrice = ShippingOption == null ? 0 : ShippingOption.Price;
            if (subTotal.Equals(0)) {
                subTotal = Subtotal();
            }
            return (
                from tax in taxes
                let name = tax.Name
                let amount = tax.ComputeTax(GetProducts(), subTotal, shippingPrice, Country, ZipCode)
                where amount > 0
                select new TaxAmount {Name = name, Amount = amount}
                ).FirstOrDefault() ?? new TaxAmount {Amount = 0, Name = null};
        }

        public decimal Total(decimal subTotal = 0, TaxAmount taxes = null) {
            if (taxes == null) {
                taxes = Taxes();
            }
            if (subTotal.Equals(0)) {
                subTotal = Subtotal();
            }
            if (taxes == null || taxes.Amount <= 0) {
                if (ShippingOption == null) return subTotal;
                return subTotal + ShippingOption.Price;
            }
            if (ShippingOption == null) return subTotal + taxes.Amount;
            return subTotal + taxes.Amount + ShippingOption.Price;
        }

        public double ItemCount() {
            return Items.Sum(x => x.Quantity);
        }

        public void Clear() {
            _products = null;
            ItemsInternal.Clear();
            UpdateItems();
        }
    }
}