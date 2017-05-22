using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Nwazet.Commerce.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Environment.Extensions;
using Orchard.Security;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.PersistentCart")]
    public class PersistentShoppingCartServices : IPersistentShoppingCartServices {

        private readonly IContentManager _contentManager;
        private readonly IWorkContextAccessor _wca;
        private readonly IEnumerable<IProductAttributeExtensionProvider> _extensionProviders;


        public PersistentShoppingCartServices(
            IContentManager contentManager,
            IWorkContextAccessor wca,
            IEnumerable<IProductAttributeExtensionProvider> extensionProviders) {

            _contentManager = contentManager;
            _wca = wca;
            _extensionProviders = extensionProviders;
        }
        private IUser User { get { return _wca.GetContext().CurrentUser; } }

        public List<ShoppingCartItem> RetrieveCartItems() {
            //get the items from the session
            var context = GetHttpContext();
            var items = (List<ShoppingCartItem>)(context.Session["ShoppingCart"]);
            if (items == null) {
                items = new List<ShoppingCartItem>();
            }
            //if we have an authenticated user, we get their cart and use that instead of the session
            if (User != null) {
                //authenticated user
                var cartPart = GetCartForUser();
                var cartPartItems = cartPart.Items;
                if (items.Count() > 0) {
                    //items were added to session while anonymous, then login happened
                    foreach (var item in items) { //merge into persistent cart
                        var existing = FindCartItem(cartPartItems, item.ProductId, item.AttributeIdsToValues);
                        if (existing != null) {
                            existing.Quantity += item.Quantity;
                        } else {
                            cartPartItems.Insert(0, item);
                        }
                    }
                    cartPart.Items = cartPartItems;
                }
                items = cartPartItems;
                context.Session["ShoppingCart"] = new List<ShoppingCartItem>(); //session is kept clear if the user is logged in to avoid duplication
            }

            // Add attribute extension providers to each attribute option
            foreach (var item in items) {
                if (item.AttributeIdsToValues != null) {
                    foreach (var option in item.AttributeIdsToValues) {
                        option.Value.ExtensionProviderInstance =
                            _extensionProviders.SingleOrDefault(e => e.Name == option.Value.ExtensionProvider);
                    }
                }
            }

            //then we return the cart from session anyway
            return items;
        }

        public ShoppingCartItem FindCartItem(IEnumerable<ShoppingCartItem> items, int productId, IDictionary<int, ProductAttributeValueExtended> attributeIdsToValues = null) {
            if (attributeIdsToValues == null || attributeIdsToValues.Count == 0) {
                return items.FirstOrDefault(i => i.ProductId == productId
                      && (i.AttributeIdsToValues == null || i.AttributeIdsToValues.Count == 0));
            }
            return items.FirstOrDefault(
                i => i.ProductId == productId
                     && i.AttributeIdsToValues != null
                     && i.AttributeIdsToValues.Count == attributeIdsToValues.Count
                     && i.AttributeIdsToValues.All(attributeIdsToValues.Contains));
        }

        public ProductsListPart GetCartForUser(IUser user = null) {
            if (user == null) {
                user = User;
            }
            if (user != null) { //must be authenticated
                var cartPart = _contentManager.Query<ProductsListPart>(VersionOptions.Latest, "ShoppingCart")
                    .Where<CommonPartRecord>(cpr => cpr.OwnerId == user.Id)
                    .Slice(1)
                    .ToList()
                    .FirstOrDefault();

                if (cartPart == null) {
                    cartPart = CreateCartForUser(user);
                }

                return cartPart;
            }
            throw new ArgumentNullException("user", "Not a valid authenticated user.");
        }

        public ProductsListPart CreateCartForUser(IUser user = null) {
            if (user == null) {
                user = User;
            }
            if (user != null) {
                var newCart = _contentManager.New("ShoppingCart");
                var commonPart = newCart.As<CommonPart>();
                commonPart.Owner = user;
                var cartPart = newCart.As<ProductsListPart>();
                cartPart.IsCart = true;
                _contentManager.Create(newCart);

                return newCart.As<ProductsListPart>();
            }
            throw new ArgumentNullException("user", "Not a valid authenticated user.");
        }

        public string Country {
            get { return PersistentCountry ?? SessionCountry; }
            set { UpdateCountry(value); }
        }
        private string PersistentCountry {
            get {
                if (User == null) {
                    return null;
                }
                return GetCartForUser().Country;
            }
        }
        private string SessionCountry {
            get { return GetHttpContext().Session["Nwazet.Country"] as string; }
        }
        private void UpdateCountry(string country) {
            UpdateSessionCountry(country);
            UpdatePersistentCountry(country);
        }
        private void UpdateSessionCountry(string country) {
            GetHttpContext().Session["Nwazet.Country"] = country;
        }
        private void UpdatePersistentCountry(string country) {
            if (User != null) {
                var cart = GetCartForUser();
                cart.Country = country;
            }
        }

        public string ZipCode {
            get { return PersistentZipCode ?? SessionZipCode; }
            set { UpdateZipCode(value); }
        }
        private string PersistentZipCode {
            get {
                if (User == null) {
                    return null;
                }
                return GetCartForUser().ZipCode;
            }
        }
        private string SessionZipCode {
            get { return GetHttpContext().Session["Nwazet.ZipCode"] as string; }
        }
        private void UpdateZipCode(string ZipCode) {
            UpdateSessionZipCode(ZipCode);
            UpdatePersistentZipCode(ZipCode);
        }
        private void UpdateSessionZipCode(string ZipCode) {
            GetHttpContext().Session["Nwazet.ZipCode"] = ZipCode;
        }
        private void UpdatePersistentZipCode(string ZipCode) {
            if (User != null) {
                var cart = GetCartForUser();
                cart.ZipCode = ZipCode;
            }
        }

        public ShippingOption ShippingOption {
            get { return PersistentShippingOption ?? SessionShippingOption; }
            set { UpdateShippingOption(value); }
        }
        private ShippingOption SessionShippingOption {
            get { return GetHttpContext().Session["Nwazet.ShippingOption"] as ShippingOption; }
        }
        private ShippingOption PersistentShippingOption {
            get {
                if (User == null) {
                    return null;
                }
                return GetCartForUser().ShippingOption;
            }
        }
        private void UpdateShippingOption(ShippingOption shippingOption) {
            UpdateSessionShippingOption(shippingOption);
            UpdatePersistentShippingOption(shippingOption);
        }
        private void UpdateSessionShippingOption(ShippingOption shippingOption) {
            GetHttpContext().Session["Nwazet.ShippingOption"] = shippingOption;
        }
        private void UpdatePersistentShippingOption(ShippingOption shippingOption) {
            if (User != null) {
                var cart = GetCartForUser();
                cart.ShippingOption = shippingOption;
            }
        }

        private HttpContextBase GetHttpContext() {
            var context = _wca.GetContext().HttpContext;
            if (context == null || context.Session == null) {
                throw new InvalidOperationException(
                    "ShoppingCartSessionStorage unavailable if session state can't be acquired.");
            }
            return context;
        }

        public void ClearCart() {
            if (User != null) {
                GetCartForUser().Items = new List<ShoppingCartItem>();
            } else {
                GetHttpContext().Session["ShoppingCart"] = new List<ShoppingCartItem>();
            }
        }

        public void ConsolidateCart() {
            var items = RetrieveCartItems().Where(it => it.Quantity > 0).ToList();
            if (User != null) {
                GetCartForUser().Items = items;
            } else {
                GetHttpContext().Session["ShoppingCart"] = items;
            }
        }

        public void RemoveItem(int productId, IDictionary<int, ProductAttributeValueExtended> attributeIdsToValues = null) {
            var items = RetrieveCartItems();
            var item = FindCartItem(items, productId, attributeIdsToValues);
            if (item != null) {
                //item is in the cart
                items.Remove(item);
                if (User != null) {
                    GetCartForUser().Items = items;
                } else {
                    GetHttpContext().Session["ShoppingCart"] = items;
                }
            }
        }

        public void AddItem(ShoppingCartItem item) {
            var items = RetrieveCartItems();
            var existing = FindCartItem(items, item.ProductId, item.AttributeIdsToValues);
            if (existing != null) {
                existing.Quantity += item.Quantity;
            } else {
                items.Insert(0, item);
            }
            if (User != null) {
                GetCartForUser().Items = items;
            } else {
                //add to session cart (session cart remains empty for logged in users)
                GetHttpContext().Session["ShoppingCart"] = items;
            }
        }
    }
}
