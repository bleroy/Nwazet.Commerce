using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Orchard;
using Orchard.ContentManagement;

namespace Nwazet.Commerce.Services {
    public interface IPromotion : IDependency {
        string Name { get; }
        IContent ContentItem { get; }

        bool Applies(
            ShoppingCartQuantityProduct quantityProduct,
            IEnumerable<ShoppingCartQuantityProduct> cartProducts);

        ShoppingCartQuantityProduct Apply(
            ShoppingCartQuantityProduct quantityProduct,
            IEnumerable<ShoppingCartQuantityProduct> cartProducts);
    }
}
