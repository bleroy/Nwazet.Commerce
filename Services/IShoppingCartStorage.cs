using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Orchard;

namespace Nwazet.Commerce.Services {
    public interface IShoppingCartStorage : IDependency {
        List<ShoppingCartItem> Retrieve();
    }
}
