using System.Collections.Generic;
using Nwazet.Commerce.Models;

namespace Nwazet.Commerce.Services {
    public interface IPriceProvider {
        IEnumerable<ShoppingCartQuantityProduct> GetModifiedPrices(
            ShoppingCartQuantityProduct quantityProduct,
            IEnumerable<ShoppingCartQuantityProduct> cartProducts);
    }
}
