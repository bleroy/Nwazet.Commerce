using Nwazet.Commerce.Services;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.UI.Notify;
using System.Collections.Generic;
using System.Linq;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Commerce")]
    public class ShoppingCart : ShoppingCartBase {
        
        public ShoppingCart(
            IContentManager contentManager,
            IShoppingCartStorage cartStorage,
            IPriceService priceService,
            IEnumerable<IProductAttributesDriver> attributesDrivers,
            IEnumerable<ITaxProvider> taxProviders,
            INotifier notifier,
            ITaxProviderService taxProviderService,
            IProductPriceService productPriceService)
            : base (contentManager,
                  cartStorage,
                  priceService,
                  attributesDrivers,
                  taxProviders,
                  notifier,
                  taxProviderService,
                  productPriceService) {
        }
        
        public override IEnumerable<ShoppingCartItem> Items {
            get { return ItemsInternal.AsReadOnly(); }
        }
        
        private List<ShoppingCartItem> ItemsInternal {
            get {
                return _cartStorage.Retrieve();
            }
        }

        public override void Add(int productId, int quantity = 1, IDictionary<int, ProductAttributeValueExtended> attributeIdsToValues = null) {
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

        public override ShoppingCartItem FindCartItem(int productId, IDictionary<int, ProductAttributeValueExtended> attributeIdsToValues = null) {
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

        public override void Remove(int productId, IDictionary<int, ProductAttributeValueExtended> attributeIdsToValues = null) {
            var item = FindCartItem(productId, attributeIdsToValues);
            if (item == null) return;

            ItemsInternal.Remove(item);
            _products = null;
        }

        public override void UpdateItems() {
            ItemsInternal.RemoveAll(x => x.Quantity <= 0);
            _products = null;
        }
        
        public override void Clear() {
            _products = null;
            ItemsInternal.Clear();
            UpdateItems();
        }
    }
}