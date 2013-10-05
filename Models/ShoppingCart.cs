using System;
using System.Collections.Generic;
using System.Linq;
using Nwazet.Commerce.Services;
using Orchard.Autoroute.Models;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Commerce")]
    public class ShoppingCart : IShoppingCart {
        private readonly IContentManager _contentManager;
        private readonly IShoppingCartStorage _cartStorage;
        private readonly IPriceService _priceService;
        private readonly IEnumerable<IProductAttributesDriver> _attributesDrivers;
        private readonly IEnumerable<ITaxProvider> _taxProviders;

        private IEnumerable<ShoppingCartQuantityProduct> _products; 

        public ShoppingCart(
            IContentManager contentManager,
            IShoppingCartStorage cartStorage,
            IPriceService priceService,
            IEnumerable<IProductAttributesDriver> attributesDrivers,
            IEnumerable<ITaxProvider> taxProviders) {

            _contentManager = contentManager;
            _cartStorage = cartStorage;
            _priceService = priceService;
            _attributesDrivers = attributesDrivers;
            _taxProviders = taxProviders;
        }

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

        public void Add(int productId, int quantity = 1, IDictionary<int, string> attributeIdsToValues = null) {
            ValidateAttributes(productId, attributeIdsToValues);
            var item = FindCartItem(productId, attributeIdsToValues);
            if (item != null) {
                item.Quantity += quantity;
            }
            else {
                ItemsInternal.Insert(0, new ShoppingCartItem(productId, quantity, attributeIdsToValues));
            }
            _products = null;
        }

        public ShoppingCartItem FindCartItem(int productId, IDictionary<int, string> attributeIdsToValues = null) {
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

        private void ValidateAttributes(int productId, IDictionary<int, string> attributeIdsToValues) {
            if (_attributesDrivers == null ||
                attributeIdsToValues == null ||
                !attributeIdsToValues.Any()) return;

            var product = _contentManager.Get(productId);
            if (_attributesDrivers.Any(d => !d.ValidateAttributes(product, attributeIdsToValues))) {
                // Throwing because this should only happen from malicious payloads
                throw new ArgumentException("Invalid product attributes", "attributeIdsToValues");
            }
        }

        public void AddRange(IEnumerable<ShoppingCartItem> items) {
            foreach (var item in items) {
                Add(item.ProductId, item.Quantity, item.AttributeIdsToValues);
            }
        }

        public void Remove(int productId, IDictionary<int, string> attributeIdsToValues = null) {
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

            var shoppingCartQuantities =
                (from item in Items
                 select new ShoppingCartQuantityProduct(item.Quantity, productParts.First(p => p.Id == item.ProductId), item.AttributeIdsToValues))
                    .ToList();

            return _products = shoppingCartQuantities
                .Select(q => _priceService.GetDiscountedPrice(q, shoppingCartQuantities))
                .ToList();
        }

        public void UpdateItems() {
            ItemsInternal.RemoveAll(x => x.Quantity <= 0);
            _products = null;
        }

        public double Subtotal() {
            return Math.Round(GetProducts().Sum(pq => Math.Round(pq.Price * pq.Quantity, 2)), 2);
        }

        public TaxAmount Taxes() {
            if (ShippingOption == null || (Country == null && ZipCode == null)) return null;
            var taxes = _taxProviders
                .SelectMany(p => p.GetTaxes())
                .OrderBy(t => t.Priority);
            return (
                from tax in taxes
                let name = tax.Name
                let amount = tax.ComputeTax(GetProducts(), Subtotal(), ShippingOption.Price, Country, ZipCode) where amount > 0
                select new TaxAmount {Name = name, Amount = amount}
                ).FirstOrDefault();
        }

        public double Total() {
            var taxes = Taxes();
            if (taxes == null) {
                if (ShippingOption == null) return Subtotal();
                return Subtotal() + ShippingOption.Price;
            }
            if (ShippingOption == null) return Subtotal() + Taxes().Amount;
            return Subtotal() + Taxes().Amount + ShippingOption.Price;
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