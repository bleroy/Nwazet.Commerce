using System.Collections.Generic;
using System.Linq;
using Nwazet.Commerce.Models;
using Orchard;
using Orchard.ContentManagement;

namespace Nwazet.Commerce.Services {
    public abstract class ProductInventoryServiceBase : IProductInventoryService {
        protected readonly IWorkContextAccessor _workContextAccessor;
        protected readonly IContentManager _contentManager;

        public ProductInventoryServiceBase(
            IWorkContextAccessor workContextAccessor,
            IContentManager contentManager) {

            _workContextAccessor = workContextAccessor;
            _contentManager = contentManager;
        }

        public virtual IEnumerable<ProductPart> GetProductsWithSameInventory(ProductPart part) {
            //return Latest and Published versions, unless they coincide or are the same as part
            var sSet = new ProductPart[] {
                _contentManager.Query<ProductPart>(VersionOptions.Published, part.ContentItem.ContentType)
                    .Where<ProductPartVersionRecord>(ppvr => ppvr.ContentItemRecord == part.Record.ContentItemRecord).List().FirstOrDefault(),
                _contentManager.Query<ProductPart>(VersionOptions.Latest, part.ContentItem.ContentType)
                    .Where<ProductPartVersionRecord>(ppvr => ppvr.ContentItemRecord == part.Record.ContentItemRecord).List().FirstOrDefault()
            };
            return sSet.Distinct().Where(lp => lp != null && lp.Record.Id != part.Record.Id);
        }

        /// <summary>
        /// Uses the inventory of the ProductPart parameter to update the ProductParts whose inventory
        /// has to be kept in synch with the parameter's.
        /// </summary>
        /// <param name="part">The ProductPart whose inventory will be copied over.</param>
        public virtual void SynchronizeInventories(ProductPart part) {
            //Synchronize inventory between Latest and Published versions
            int inv = GetInventory(part);
            foreach (var pp in
               GetProductsWithSameInventory(part)
                   .Where(pa => GetInventory(pa) != inv)) { //condition to avoid infinite recursion
                SetInventory(pp, GetInventory(part)); //call methods from base class
            }
            //Synchronize the inventory for the eventual bundles that contain the product
            IBundleService bundleService;
            if (_workContextAccessor.GetContext().TryResolve(out bundleService)) {
                var affectedBundles = _contentManager.Query<BundlePart, BundlePartRecord>()
                    .Where(b => b.Products.Any(p => p.ContentItemRecord.Id == part.Id))
                    .WithQueryHints(new QueryHints().ExpandParts<ProductPart>())
                    .List();
                foreach (var bundle in affectedBundles.Where(b => b.ContentItem.As<ProductPart>() != null)) {
                    var prod = bundle.ContentItem.As<ProductPart>();
                    SetInventory(prod, GetInventory(prod));
                }
            }
        }

        public virtual int SetInventory(ProductPart part, int inventoryValue) {
            part.Inventory = inventoryValue;
            SynchronizeInventories(part);
            return part.Inventory;
        }

        public virtual int UpdateInventory(ProductPart part, int inventoryChange) {
            part.Inventory += inventoryChange;
            SynchronizeInventories(part);
            return part.Inventory;
        }

        public virtual int GetInventory(ProductPart part) {
            IBundleService bundleService;
            var inventory = part.Inventory;
            if (_workContextAccessor.GetContext().TryResolve(out bundleService) && part.Has<BundlePart>()) {
                var bundlePart = part.As<BundlePart>();
                var ids = bundlePart.ProductIds.ToList();
                if (!ids.Any()) return 0;
                inventory =
                    bundleService
                        .GetProductQuantitiesFor(bundlePart)
                        .Min(p => p.Product.Inventory / p.Quantity);
            }
            return inventory;
        }

        public virtual IEnumerable<ProductPart> GetProductsWithInventoryIssues() {
            return new List<ProductPart>(); //There is no criteria here to find issues
        }
    }
}
