using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.PersistentCart")]
    public class ShoppingCartInfosetPartStorage : IShoppingCartStorage {
        private readonly IWorkContextAccessor _wca;
        private readonly IEnumerable<IProductAttributeExtensionProvider> _extensionProviders;
        private readonly IContentManager _contentManager;
        private readonly IPersistentShoppingCartServices _persistentShoppingCartServices;

        public ShoppingCartInfosetPartStorage(
            IWorkContextAccessor wca,
            IEnumerable<IProductAttributeExtensionProvider> extensionProviders,
            IContentManager contentManager,
            IPersistentShoppingCartServices persistentShoppingCartServices) {

            _wca = wca;
            _extensionProviders = extensionProviders;
            _contentManager = contentManager;
            _persistentShoppingCartServices = persistentShoppingCartServices;
        }

        public string Country
        {
            get
            {
                var cart = GetCart();
                return cart == null ? "" : cart.Country;
            }

            set
            {
                _persistentShoppingCartServices.UpdateCountry(GetCart(), value);
            }
        }

        public ShippingOption ShippingOption
        {
            get
            {
                var cart = GetCart();
                return cart == null ? null : cart.ShippingOption;
            }

            set
            {
                _persistentShoppingCartServices.UpdateShippingOption(GetCart(), value);
            }
        }

        public string ZipCode
        {
            get
            {
                var cart = GetCart();
                return cart == null ? "" : cart.ZipCode;
            }

            set
            {
                _persistentShoppingCartServices.UpdateZipCode(GetCart(), value);
            }
        }

        public List<ShoppingCartItem> Retrieve() {
            var cart = GetCart();
            return cart == null ? new List<ShoppingCartItem>() : cart.Items;
        }

        private PersistentShoppingCartPart GetCart() {
            //get the cart for the current user, if he's logged in. 
            var user = _wca.GetContext().CurrentUser;
            if (user != null) {
                //authenticated user
                return _persistentShoppingCartServices.GetCartForUser(user);
            }
            return _persistentShoppingCartServices.GetAnonymousCart();
        }
    }
}
