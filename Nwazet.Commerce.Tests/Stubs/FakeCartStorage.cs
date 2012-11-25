using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;

namespace Nwazet.Commerce.Tests.Stubs {
    public class FakeCartStorage : IShoppingCartStorage {
        private readonly List<ShoppingCartItem> _cartItems;
 
        public FakeCartStorage() {
            _cartItems = new List<ShoppingCartItem>();
        }

        public List<ShoppingCartItem> Retrieve() {
            return _cartItems;
        }
    }
}
