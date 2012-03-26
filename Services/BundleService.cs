using System.Collections.Generic;
using System.Linq;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Services {
    public interface IBundleService : IDependency {
        void UpdateBundleProducts(ContentItem item, IEnumerable<ProductEntry> products);
        IEnumerable<IContent> GetProducts();
        BundleViewModel BuildEditorViewModel(BundlePart part);
        void AddProduct(int product, BundlePartRecord record);
    }

    [OrchardFeature("Nwazet.Bundles")]
    public class BundleService : IBundleService {
        private readonly IContentManager _contentManager;
        private readonly IRepository<BundleProductsRecord> _bundleProductsRepository;

        public BundleService(
            IContentManager contentManager,
            IRepository<BundleProductsRecord> bundleProductsRepository) {

            _contentManager = contentManager;
            _bundleProductsRepository = bundleProductsRepository;
        }

        public void UpdateBundleProducts(ContentItem item, IEnumerable<ProductEntry> products) {
            var record = item.As<BundlePart>().Record;
            var oldProducts = _bundleProductsRepository.Fetch(
                r => r.BundlePartRecord == record);
            var lookupNew = products
                .Where(e => e.IsChecked)
                .Select(e => e.ProductId)
                .ToDictionary(r => r, r => false);
            // Delete the products that are no longer there
            // and mark the ones that should stay
            foreach (var bundleProductRecord in oldProducts) {
                if (lookupNew.ContainsKey(bundleProductRecord.ContentItemRecord.Id)) {
                    lookupNew[bundleProductRecord.ContentItemRecord.Id] = true;
                }
                else {
                    _bundleProductsRepository.Delete(bundleProductRecord);
                }
            }
            // Add the new products
            foreach (var product in lookupNew
                .Where(kvp => !kvp.Value)
                .Select(kvp => kvp.Key)) {

                AddProduct(product, record);
            }
        }

        public void AddProduct(int product, BundlePartRecord record) {
            _bundleProductsRepository.Create(
                new BundleProductsRecord {
                    BundlePartRecord = record,
                    ContentItemRecord = _contentManager.Get(product).Record
                });
        }

        public IEnumerable<IContent> GetProducts() {
            return _contentManager
                .List<ProductPart>()
                .Where(p => !p.Has<BundlePart>());
        }

        public BundleViewModel BuildEditorViewModel(BundlePart part) {
            var bundleProducts = part.ProductIds.ToLookup(p => p);
            return new BundleViewModel {
                Products = GetProducts()
                    .Select(
                        p => new ProductEntry {
                            ProductId = p.ContentItem.Id,
                            Product = p,
                            IsChecked = bundleProducts.Contains(p.Id),
                            DisplayText = _contentManager.GetItemMetadata(p).DisplayText
                        }
                    ).ToList()
            };
        }
    }
}
