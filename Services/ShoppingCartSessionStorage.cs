using System;
using System.Collections.Generic;
using System.Web;
using Nwazet.Commerce.Models;
using Orchard;

namespace Nwazet.Commerce.Services {
    public class ShoppingCartSessionStorage : IShoppingCartStorage {
        private readonly IWorkContextAccessor _wca;

        public ShoppingCartSessionStorage(IWorkContextAccessor wca) {
            _wca = wca;
        }

        public List<ShoppingCartItem> Retrieve() {
            var context = GetHttpContext();
            var items = (List<ShoppingCartItem>) (context.Session["ShoppingCart"]);

            if (items == null) {
                items = new List<ShoppingCartItem>();
                context.Session["ShoppingCart"] = items;
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
