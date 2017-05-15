using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Environment.Extensions;
using Orchard.Security;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.PersistentCart")]
    public class PersistentShoppingCartServices : IPersistentShoppingCartServices {

        private readonly IContentManager _contentManager;
        private readonly IAnonymousIdentityProvider _anonymousIdentityProvider;

        public PersistentShoppingCartServices(
            IContentManager contentManager,
            IAnonymousIdentityProvider anonymousIdentityProvider) {

            _contentManager = contentManager;
            _anonymousIdentityProvider = anonymousIdentityProvider;
        }

        public PersistentShoppingCartPart GetCartForUser(IUser user) {
            if (user != null) { //must be authenticated
                var cartPart = _contentManager.Query<PersistentShoppingCartPart>(VersionOptions.Latest, "ShoppingCart")
                    .Where<CommonPartRecord>(cpr => cpr.OwnerId == user.Id)
                    .Slice(1)
                    .ToList()
                    .FirstOrDefault();

                if (cartPart == null) {
                    cartPart = CreateCartForUser(user);
                }

                return cartPart;
            }

            return GetAnonymousCart();
        }

        public PersistentShoppingCartPart CreateCartForUser(IUser user) {
            if (user != null) {
                var newCart = _contentManager.New("ShoppingCart");
                var commonPart = newCart.As<CommonPart>();
                commonPart.Owner = user;
                var cartPart = newCart.As<PersistentShoppingCartPart>();
                _contentManager.Create(newCart);

                return newCart.As<PersistentShoppingCartPart>();
            }
            throw new ArgumentNullException("user", "Not a valid authenticated user.");
        }

        public PersistentShoppingCartPart GetAnonymousCart() {
            var identity = _anonymousIdentityProvider.GetAnonymousIdentifier();
            var cartPart = _contentManager.Query<PersistentShoppingCartPart>(VersionOptions.Latest, "ShoppingCart")
                .Where<NamedProductsListRecord>(cpr => cpr.AnonymousId == identity)
                .Slice(1)
                .ToList()
                .FirstOrDefault();

            if (cartPart == null) {
                cartPart = CreateAnonymousCart(identity);
            }

            return cartPart;
        }

        public PersistentShoppingCartPart CreateAnonymousCart(string identity) {
            var newCart = _contentManager.New("ShoppingCart");
            var cartPart = newCart.As<PersistentShoppingCartPart>();
            cartPart.AnonymousId = identity;
            _contentManager.Create(newCart);

            return newCart.As<PersistentShoppingCartPart>();
        }


        public PersistentShoppingCartPart UpdateCountry(PersistentShoppingCartPart cart, string country) {
            if (cart == null) {
                return null;
            }
            cart.Country = country;
            _contentManager.Create(cart, VersionOptions.DraftRequired);
            return cart;
        }

        public PersistentShoppingCartPart UpdateZipCode(PersistentShoppingCartPart cart, string zipCode) {
            if (cart == null) {
                return null;
            }
            cart.ZipCode = zipCode;
            _contentManager.Create(cart, VersionOptions.DraftRequired);
            return cart;
        }

        public PersistentShoppingCartPart UpdateShippingOption(PersistentShoppingCartPart cart, ShippingOption shippingOption) {
            if (cart == null) {
                return null;
            }
            cart.ShippingOption = shippingOption;
            _contentManager.Create(cart, VersionOptions.DraftRequired);
            return cart;
        }
    }
}
