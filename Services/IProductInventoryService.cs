using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
using Orchard;

namespace Nwazet.Commerce.Services {
    public interface IProductInventoryService : IDependency {
        /// <summary>
        /// Get all the ProductParts that share the inventory with the one passed as parameter.
        /// </summary>
        /// <param name="part">The ProductPart for which we wish to find the products that share its inventory.</param>
        /// <returns>A IEnumerable of the ProductParts sharing the same inventory as the one we passed.</returns>
        IEnumerable<ProductPart> GetProductsWithSameInventory(ProductPart part);
        /// <summary>
        /// Copies the inventory of the ProductPart parameter to all the ProductParts whose inventory
        /// has to be kept in synch with the parameter's.
        /// </summary>
        /// <param name="part">The ProductPart whose inventory will be copied over.</param>
        void SynchronizeInventories(ProductPart part);
    }
}
