using System;
using System.Collections.Generic;
using System.Linq;
using Nwazet.Commerce.Services;
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

        public ShoppingCart(
            IContentManager contentManager,
            IShoppingCartStorage cartStorage,
            IPriceService priceService,
            IEnumerable<IProductAttributesDriver> attributesDrivers) {

            _contentManager = contentManager;
            _cartStorage = cartStorage;
            _priceService = priceService;
            _attributesDrivers = attributesDrivers;
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
        }

        public IEnumerable<ShoppingCartQuantityProduct> GetProducts() {
            var ids = Items.Select(x => x.ProductId);

            var productParts =
                _contentManager.GetMany<ProductPart>(ids, VersionOptions.Published, new QueryHints().ExpandParts<TitlePart>()).ToArray();

            var shoppingCartQuantities =
                (from item in Items
                 select new ShoppingCartQuantityProduct(item.Quantity, productParts.First(p => p.Id == item.ProductId), item.AttributeIdsToValues))
                    .ToList();

            return shoppingCartQuantities
                .Select(q => _priceService.GetDiscountedPrice(q, shoppingCartQuantities));
        }

        public void UpdateItems() {
            ItemsInternal.RemoveAll(x => x.Quantity <= 0);
        }

        public double Subtotal() {
            return Math.Round(GetProducts().Sum(pq => Math.Round(pq.Price * pq.Quantity, 2)), 2);
        }

        public double Taxes() {
            // TODO: handle taxes
            return 0;
        }

        public double Total() {
            return Subtotal() + Taxes();
        }

        public double ItemCount() {
            return Items.Sum(x => x.Quantity);
        }

        public void Clear() {
            ItemsInternal.Clear();
            UpdateItems();
        }
    }
}