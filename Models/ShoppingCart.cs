using System;
using System.Collections.Generic;
using System.Linq;
using Nwazet.Commerce.Services;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Commerce")]
    public class ShoppingCart : IShoppingCart {
        private readonly IContentManager _contentManager;
        private readonly IShoppingCartStorage _cartStorage;
        private readonly IEnumerable<IPriceProvider> _priceProviders;
        private const double Epsilon = 0.001;

        public ShoppingCart(
            IContentManager contentManager,
            IShoppingCartStorage cartStorage,
            IEnumerable<IPriceProvider> priceProviders) {

            _contentManager = contentManager;
            _cartStorage = cartStorage;
            _priceProviders = priceProviders;
        }

        public IEnumerable<ShoppingCartItem> Items {
            get { return ItemsInternal.AsReadOnly(); }
        }

        private List<ShoppingCartItem> ItemsInternal {
            get {
                return _cartStorage.Retrieve();
            }
        }

        public void Add(int productId, int quantity = 1) {
            var item = Items.SingleOrDefault(x => x.ProductId == productId);

            if (item == null) {
                item = new ShoppingCartItem(productId, quantity);
                ItemsInternal.Add(item);
            }
            else {
                item.Quantity += quantity;
            }
        }

        public void AddRange(IEnumerable<ShoppingCartItem> items) {
            foreach (var item in items) {
                Add(item.ProductId, item.Quantity);
            }
        }

        public void Remove(int productId) {
            var item = Items.SingleOrDefault(x => x.ProductId == productId);
            if (item == null) return;

            ItemsInternal.Remove(item);
        }

        public ProductPart GetProduct(int productId) {
            return _contentManager.Get(productId).As<ProductPart>();
        }

        public IEnumerable<ShoppingCartQuantityProduct> GetProducts() {
            var ids = Items.Select(x => x.ProductId);

            var productParts =
                _contentManager.GetMany<ProductPart>(ids, VersionOptions.Latest, QueryHints.Empty).ToArray();

            var shoppingCartQuantities =
                (from item in Items
                 from product in productParts
                 where product.Id == item.ProductId
                 select new ShoppingCartQuantityProduct(item.Quantity, product))
                    .ToList();

            return shoppingCartQuantities
                .Select(
                    q => {
                        var modifiedPrices = _priceProviders
                            .SelectMany(pp => pp.GetModifiedPrices(q, shoppingCartQuantities))
                            .ToList();
                        if (!modifiedPrices.Any()) return q;
                        var minPrice = modifiedPrices.Min(mp => mp.Price);
                        q.Price = minPrice;
                        var lowestPrice = modifiedPrices.FirstOrDefault(mp => Math.Abs(mp.Price - minPrice) < Epsilon);
                        if (lowestPrice != null) {
                            q.Comment = lowestPrice.Comment;
                        }
                        return q;
                    });
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