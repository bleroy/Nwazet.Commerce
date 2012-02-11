using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Commerce")]
    public class ShoppingCart : IShoppingCart {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IContentManager _contentManager;
        private readonly IOrchardServices _orchardServices;

        public ShoppingCart(
            IWorkContextAccessor workContextAccessor,
            IContentManager contentManager,
            IOrchardServices orchardServices) {

            _workContextAccessor = workContextAccessor;
            _contentManager = contentManager;
            _orchardServices = orchardServices;
        }

        public IEnumerable<ShoppingCartItem> Items {
            get { return ItemsInternal.AsReadOnly(); }
        }

        private HttpContextBase HttpContext {
            get { return _workContextAccessor.GetContext().HttpContext; }
        }

        private List<ShoppingCartItem> ItemsInternal {
            get {
                var items = (List<ShoppingCartItem>) HttpContext.Session["ShoppingCart"];

                if (items == null) {
                    items = new List<ShoppingCartItem>();
                    HttpContext.Session["ShoppingCart"] = items;
                }

                return items;
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
                _orchardServices.ContentManager.GetMany<ProductPart>(ids, VersionOptions.Latest, QueryHints.Empty).ToArray();

            var query = from item in Items
                        from product in productParts
                        where product.Id == item.ProductId
                        select new ShoppingCartQuantityProduct(item.Quantity, product);

            return query;
        }

        public void UpdateItems() {
            ItemsInternal.RemoveAll(x => x.Quantity <= 0);
        }

        public double Subtotal() {
            return GetProducts().Sum(pq => pq.Product.Price * pq.Quantity);
        }

        public double Taxes() {
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