using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Handlers {
    [OrchardFeature("Nwazet.Commerce")]
    public class ProductPartVersioningHandler : ContentHandler {
        private readonly IContentManager _contentManager;
        private readonly IProductInventoryService _productInventoryService;
        public ProductPartVersioningHandler(
            IRepository<ProductPartVersionRecord> versionRepository,
            IContentManager contentManager,
            IProductInventoryService productInventoryService
            ) {

            Filters.Add(StorageFilter.For(versionRepository));

            _contentManager = contentManager;
            _productInventoryService = productInventoryService;
            
            OnUpdated<ProductPart>(SynchronizeOnUpdate);
        }

        protected void SynchronizeOnUpdate(UpdateContentContext context, ProductPart part) {
            //The Inventory gets copied over to Latest and Published
            _productInventoryService.SynchronizeInventories(part);
        }

        private void SynchronizeInventory(ProductPart part, IEnumerable<ProductPart> targets) {
            foreach (var target in targets) {
                target.Inventory = part.Inventory;
            }
        }

        private IEnumerable<ProductPart> GetSynchronizationSet(ProductPart part) {
            //return Latest and Published versions, unless they coincide or are the same as part
            var sSet = new ProductPart[] {
                _contentManager.Query<ProductPart>(VersionOptions.Published, part.ContentItem.ContentType)
                    .Where<ProductPartVersionRecord>(ppvr => ppvr.ContentItemRecord == part.Record.ContentItemRecord).List().FirstOrDefault(),
                _contentManager.Query<ProductPart>(VersionOptions.Latest, part.ContentItem.ContentType)
                    .Where<ProductPartVersionRecord>(ppvr => ppvr.ContentItemRecord == part.Record.ContentItemRecord).List().FirstOrDefault()
            };

            return sSet.Distinct().Where(lp => lp != null && lp.Record.Id != part.Record.Id);
        }
    }
}
