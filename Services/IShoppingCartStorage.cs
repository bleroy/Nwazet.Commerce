using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Orchard;

namespace Nwazet.Commerce.Services {
    public interface IShoppingCartStorage : IDependency {
        List<ShoppingCartItem> Retrieve();
        string Country { get; set; }
        string ZipCode { get; set; }
        ShippingOption ShippingOption { get; set; }
    }
}
