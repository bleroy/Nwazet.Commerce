using System;
using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Orchard;

namespace Nwazet.Commerce.Services {
    public class ShoppingCartSessionStorage : IShoppingCartStorage {
        private readonly IWorkContextAccessor _wca;

        public ShoppingCartSessionStorage(IWorkContextAccessor wca) {
            _wca = wca;
        }

        public List<ShoppingCartItem> Retrieve() {
            var context = _wca.GetContext().HttpContext;
            if (context == null || context.Session == null) {
                throw new InvalidOperationException("ShoppingCartSessionStorage unavailable if session state can't be acquired.");
            }
            var items = (List<ShoppingCartItem>)(context.Session["ShoppingCart"]);

            if (items == null) {
                items = new List<ShoppingCartItem>();
                context.Session["ShoppingCart"] = items;
            }
            return items;
        }
    }
}
