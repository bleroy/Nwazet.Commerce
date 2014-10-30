using System;
using System.Collections.Generic;
using System.Web;
using Nwazet.Commerce.Models;
using Orchard;
using System.Linq;

namespace Nwazet.Commerce.Services {
    public class ShoppingCartSessionStorage : IShoppingCartStorage {
        private readonly IWorkContextAccessor _wca;
        private readonly IEnumerable<IProductAttributeExtensionProvider> _extensionProviders;

        public ShoppingCartSessionStorage(
            IWorkContextAccessor wca,
            IEnumerable<IProductAttributeExtensionProvider> extensionProviders) {
            _wca = wca;
            _extensionProviders = extensionProviders;
        }

        public List<ShoppingCartItem> Retrieve() {
            var context = GetHttpContext();
            var items = (List<ShoppingCartItem>) (context.Session["ShoppingCart"]);

            if (items == null) {
                items = new List<ShoppingCartItem>();
                context.Session["ShoppingCart"] = items;
            } else {
                // Add attribute extension providers to each attribute option
                foreach (var item in items) {
                    foreach (var option in item.AttributeIdsToValues) {
                        option.Value.ExtensionProviderInstance = _extensionProviders.SingleOrDefault(e => e.Name == option.Value.ExtensionProvider);
                    }
                }
            }
            return items;
        }

        public string Country {
            get {
                var context = GetHttpContext();
                return context.Session["Nwazet.Country"] as string;
            }
            set {
                var context = GetHttpContext();
                context.Session["Nwazet.Country"] = value;
            }
        }

        public string ZipCode {
            get {
                var context = GetHttpContext();
                return context.Session["Nwazet.ZipCode"] as string;
            }
            set {
                var context = GetHttpContext();
                context.Session["Nwazet.ZipCode"] = value;
            }
        }

        public ShippingOption ShippingOption {
            get {
                var context = GetHttpContext();
                return context.Session["Nwazet.ShippingOption"] as ShippingOption;
            }
            set {
                var context = GetHttpContext();
                context.Session["Nwazet.ShippingOption"] = value;
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
    }
}
