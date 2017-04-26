using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;

namespace Nwazet.Commerce.Services {
    public abstract class ProductInventoryServiceBase : IProductInventoryService {

        public virtual IEnumerable<ProductPart> GetProductsWithSameInventory(ProductPart part) {
            return new List<ProductPart>(); //do nothing
        }

        public virtual void SynchronizeInventories(ProductPart part) {
            //do nothing 
        }

        public virtual int UpdateInventory(ProductPart part, int inventoryChange) {
            part.Inventory += inventoryChange;
            SynchronizeInventories(part);
            return part.Inventory;
        }
    }
}
