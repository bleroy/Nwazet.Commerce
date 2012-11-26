using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Orchard;

namespace Nwazet.Commerce.Services {
    public interface IPriceProvider : IDependency {
        string Name { get; }
        string ContentTypeName { get; }

        IEnumerable<IPromotion> GetPromotions();

        IEnumerable<ShoppingCartQuantityProduct> GetModifiedPrices(
            ShoppingCartQuantityProduct quantityProduct,
            IEnumerable<ShoppingCartQuantityProduct> cartProducts);
    }
}
