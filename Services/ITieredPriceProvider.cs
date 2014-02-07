using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Orchard;

namespace Nwazet.Commerce.Services {
    public interface ITieredPriceProvider : IDependency {
        IEnumerable<PriceTier> GetPriceTiers(ProductPart product);
        ShoppingCartQuantityProduct GetTieredPrice(ShoppingCartQuantityProduct quantityProduct);
    }
}
