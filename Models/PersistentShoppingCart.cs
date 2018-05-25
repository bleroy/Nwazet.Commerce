using Nwazet.Commerce.Services;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.UI.Notify;
using System.Collections.Generic;
using System.Linq;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.PersistentCart")]
    public class PersistentShoppingCart : ShoppingCartBase {
        
        private readonly IPersistentShoppingCartServices _persistentShoppingCartServices;
        
        public PersistentShoppingCart(
            IContentManager contentManager,
            IShoppingCartStorage cartStorage,
            IPriceService priceService,
            IEnumerable<IProductAttributesDriver> attributesDrivers,
            IEnumerable<ITaxProvider> taxProviders,
            INotifier notifier,
            IPersistentShoppingCartServices persistentShoppingCartServices,
            ITaxProviderService taxProviderService,
            IProductPriceService productPriceService)
            : base(contentManager,
                cartStorage,
                priceService,
                attributesDrivers,
                taxProviders,
                notifier,
                taxProviderService,
                productPriceService) {
            
            _persistentShoppingCartServices = persistentShoppingCartServices;
        }
        
        public override IEnumerable<ShoppingCartItem> Items {
            get { return _cartStorage.Retrieve(); }
        }

        public override ShoppingCartItem FindCartItem(int productId, IDictionary<int, ProductAttributeValueExtended> attributeIdsToValues = null) {
            return _persistentShoppingCartServices.FindCartItem(Items, productId, attributeIdsToValues);
        }

        private bool ValidateAttributes(int productId, IDictionary<int, ProductAttributeValueExtended> attributeIdsToValues) {
            if (_attributesDrivers == null ||
                attributeIdsToValues == null ||
                !attributeIdsToValues.Any()) return true;

            var product = _contentManager.Get(productId);
            return _attributesDrivers.All(d => d.ValidateAttributes(product, attributeIdsToValues));
        }

        public override void Add(int productId, int quantity = 1, IDictionary<int, ProductAttributeValueExtended> attributeIdsToValues = null) {
            if (!ValidateAttributes(productId, attributeIdsToValues)) {
                // If attributes don't validate, don't add the product, but notify
                _notifier.Warning(T("Couldn't add this product because of invalid attributes. Please refresh the page and try again."));
                return;
            }

            _persistentShoppingCartServices.AddItem(new ShoppingCartItem(productId, quantity, attributeIdsToValues));

            _products = null;
        }

        public override void Clear() {
            _products = null;
            _persistentShoppingCartServices.ClearCart();
            UpdateItems();
        }

        public override void Remove(int productId, IDictionary<int, ProductAttributeValueExtended> attributeIdsToValues = null) {
            _persistentShoppingCartServices.RemoveItem(productId, attributeIdsToValues);
            _products = null;
        }

        public override void UpdateItems() {
            _persistentShoppingCartServices.ConsolidateCart();
            _products = null;
        }
        
    }
}
