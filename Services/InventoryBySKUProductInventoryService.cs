using System.Collections.Generic;
using System.Linq;
using Nwazet.Commerce.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.InventoryBySKU")]
    public class InventoryBySKUProductInventoryService : ProductInventoryServiceBase {

        public InventoryBySKUProductInventoryService(
            IWorkContextAccessor workContextAccessor,
            IContentManager contentManager)
            : base(workContextAccessor, contentManager) { }

        public override IEnumerable<ProductPart> GetProductsWithSameInventory(ProductPart part) {
            return _contentManager
                .Query<ProductPart, ProductPartRecord>(VersionOptions.Latest)
                .Where(pa => pa.Sku == part.Sku && pa.ContentItemRecord.Id != part.ContentItem.Id)
                .List();
        }

        public override void SynchronizeInventories(ProductPart part) {
            base.SynchronizeInventories(part); //call this to synchronize bundles
            int inv = GetInventory(part);
            foreach (var pp in
                GetProductsWithSameInventory(part)
                    .Where(pa => GetInventory(pa) != inv)) { //condition to avoid infinite recursion
                SetInventory(pp, GetInventory(part)); //call methods from base class
            }
        }
    }
}
