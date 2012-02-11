using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Orchard;

namespace Nwazet.Commerce.Services {
    public interface ICheckoutService : IDependency {
        dynamic BuildCheckoutButtonShape(IEnumerable<dynamic> productShapes, IEnumerable<ShoppingCartQuantityProduct> productQuantities);
    }
}
