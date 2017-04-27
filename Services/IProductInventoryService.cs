using System.Collections.Generic;
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
        /// Uses the inventory of the ProductPart parameter to update the ProductParts whose inventory
        /// has to be kept in synch with the parameter's.
        /// </summary>
        /// <param name="part">The ProductPart whose inventory will be copied over.</param>
        void SynchronizeInventories(ProductPart part);
        /// <summary>
        /// Updates the value of the ProductPart's inventory by adding the input value.
        /// This method calls SynchronizeInventories after updating the part's inventory.
        /// </summary>
        /// <param name="part">The ProductPart whose inventory we are changing.</param>
        /// <param name="inventoryChange">The change to the inventory. This may be negative to decrease the inventory.</param>
        /// <returns>The new value of the ProductPart's inventory.</returns>
        int UpdateInventory(ProductPart part, int inventoryChange);
        /// <summary>
        /// Get the inventory of the ProductPart specified.
        /// </summary>
        /// <param name="part">The ProductPart whose inventory we are looking for.</param>
        /// <returns>The quantity in inventory.</returns>
        int GetInventory(ProductPart part);
    }
}
