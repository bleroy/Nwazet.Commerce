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
                .Query<ProductPart, ProductPartVersionRecord>(VersionOptions.Latest)
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

        public override IEnumerable<ProductPart> GetProductsWithInventoryIssues() {
            var badProducts = //new List<ProductPart>();
            _contentManager
            .Query<ProductPart, ProductPartVersionRecord>(VersionOptions.Latest)
            .List() //Get all ProductParts
            .GroupBy(pp => pp.Sku) //group them based on their SKU
            .Where(group => group.Count() > 1) //single products are not groups
            .Where(group => group
                .Select(pp => GetInventory(pp))
                .Distinct()
                .Count() > 1) //groups where the inventories are not all the same
            .Select(group => group.First()); //get the first ProductPart as representative of the group

            return badProducts;
        }
    }
}
