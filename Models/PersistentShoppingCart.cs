using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Services;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.UI.Notify;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.PersistentCart")]
    public class PersistentShoppingCart : ShoppingCart {

        private readonly IContentManager _contentManager;
        private readonly IShoppingCartStorage _cartStorage;
        private readonly IPriceService _priceService;
        private readonly IEnumerable<IProductAttributesDriver> _attributesDrivers;
        private readonly IEnumerable<ITaxProvider> _taxProviders;
        private readonly INotifier _notifier;

        public PersistentShoppingCart(
            IContentManager contentManager,
            IShoppingCartStorage cartStorage,
            IPriceService priceService,
            IEnumerable<IProductAttributesDriver> attributesDrivers,
            IEnumerable<ITaxProvider> taxProviders,
            INotifier notifier) 
            : base (contentManager, cartStorage, priceService, attributesDrivers, taxProviders, notifier) {

            _contentManager = contentManager;
            _cartStorage = cartStorage;
            _priceService = priceService;
            _attributesDrivers = attributesDrivers;
            _taxProviders = taxProviders;
            _notifier = notifier;

            T = NullLocalizer.Instance;
        }
        
        
        public new void Add(int productId, int quantity = 1, IDictionary<int, ProductAttributeValueExtended> attributeIdsToValues = null) {
            if (!ValidateAttributes(productId, attributeIdsToValues)) {
                // If attributes don't validate, don't add the product, but notify
                _notifier.Warning(T("Couldn't add this product because of invalid attributes. Please refresh the page and try again."));
                return;
            }

            var item = FindCartItem(productId, attributeIdsToValues);
            if (item != null) {
                //TODO: update quantity for item
            }
            else {
                //TODO: add a new item
            }
            _products = null;

            throw new NotImplementedException();
        }

        public new void Clear() {
            _products = null;
            //TODO: empty the cart
            UpdateItems();
        }
        
        public new void Remove(int productId, IDictionary<int, ProductAttributeValueExtended> attributeIdsToValues = null) {
            var item = FindCartItem(productId, attributeIdsToValues);
            if (item == null) return;

            //TODO: actually remove the item
            _products = null;
        }
        
        public new void UpdateItems() {
            //TODO: remove all elements whose quantity is zero
            _products = null;
        }
    }
}
