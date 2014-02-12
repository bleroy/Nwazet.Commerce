using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Orchard;

namespace Nwazet.Commerce.Services {
    public interface IPriceService : IDependency {
        ShoppingCartQuantityProduct GetDiscountedPrice(
            ShoppingCartQuantityProduct productQuantity,
            IEnumerable<ShoppingCartQuantityProduct> shoppingCartQuantities = null);
        IEnumerable<PriceTier> GetDiscountedPriceTiers(ProductPart product);
    }
}
