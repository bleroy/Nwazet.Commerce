using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.InventoryBySKU")]
    public class InventoryBySKUProductInventoryService : IProductInventoryService {
        private readonly IContentManager _contentManager;
        public InventoryBySKUProductInventoryService(
            IContentManager contentManager) {

            _contentManager = contentManager;
        }
        public IEnumerable<ProductPart> GetProductsWithSameInventory(ProductPart part) {
            return _contentManager
                .Query<ProductPart, ProductPartRecord>(VersionOptions.Latest)
                .Where(pa => pa.Sku == part.Sku && pa.ContentItemRecord.Id != part.ContentItem.Id)
                .List();
        }

        public void SynchronizeInventories(ProductPart part) {
            foreach (var pp in GetProductsWithSameInventory(part)) {
                pp.Inventory = part.Inventory;
            }
        }
    }
}
