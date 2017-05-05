using System;
using System.Collections.Generic;
using System.Linq;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.ViewModels;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.MediaLibrary.Models;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.Bundles")]
    public abstract class BundleServiceBase : IBundleService {
        private readonly IContentManager _contentManager;
        private readonly IRepository<BundleProductsRecord> _bundleProductsRepository;

        public BundleServiceBase (
           IContentManager contentManager,
           IRepository<BundleProductsRecord> bundleProductsRepository) {

            _contentManager = contentManager;
            _bundleProductsRepository = bundleProductsRepository;
        }

        public virtual void AddProduct(int quantity, int product, BundlePartRecord record) {
            _bundleProductsRepository.Create(
                new BundleProductsRecord {
                    BundlePartRecord = record,
                    ContentItemRecord = _contentManager.Get(product, VersionOptions.Latest).Record,
                    Quantity = quantity
                });
        }

        public virtual BundleViewModel BuildEditorViewModel(BundlePart part) {
            var bundleProductQuantities = part.ProductQuantities.ToDictionary(pq => pq.ProductId, pq => pq.Quantity);
            return new BundleViewModel {
                Products = GetProducts()
                    .Select(
                        p => {
                            var id = p.ContentItem.Id;
                            return new ProductEntry {
                                ProductId = id,
                                Product = p,
                                Quantity = bundleProductQuantities.ContainsKey(id) ? bundleProductQuantities[id] : 0,
                                DisplayText = _contentManager.GetItemMetadata(p).DisplayText
                            };
                        }
                    )
                    .OrderBy(vm => vm.DisplayText)
                    .ToList()
            };
        }

        public virtual IEnumerable<ProductPartQuantity> GetProductQuantitiesFor(BundlePart bundle) {
            var quantities = bundle.ProductQuantities.ToDictionary(q => q.ProductId, q => q.Quantity);
            var queryHints = new QueryHints()
                .ExpandRecords<TitlePartRecord, MediaPartRecord>();
            var parts = _contentManager.GetMany<ProductPart>(quantities.Keys, VersionOptions.Published, queryHints);
            return parts.Select(p => new ProductPartQuantity {
                Product = p,
                Quantity = quantities[p.ContentItem.Id]
            }).ToList();
        }

        public virtual IEnumerable<IContent> GetProducts() {
            return _contentManager
                .Query<ProductPart, ProductPartRecord>(VersionOptions.Latest)
                .List()
                .Where(p => !p.Has<BundlePart>());
        }

        public virtual void UpdateBundleProducts(ContentItem item, IEnumerable<ProductEntry> products) {
            var record = item.As<BundlePart>().Record;
            var oldProducts = _bundleProductsRepository
                .Fetch(r => r.BundlePartRecord == record)
                .ToList();
            var lookupNew = products
                .Where(e => e.Quantity > 0)
                .ToDictionary(r => r.ProductId, r => r.Quantity);
            // Delete the products that are no longer there
            // and updtes the ones that should stay
            foreach (var bundleProductRecord in oldProducts) {
                var key = bundleProductRecord.ContentItemRecord.Id;
                if (lookupNew.ContainsKey(key)) {
                    bundleProductRecord.Quantity = lookupNew[key];
                    _bundleProductsRepository.Update(bundleProductRecord);
                    lookupNew.Remove(key);
                }
                else {
                    _bundleProductsRepository.Delete(bundleProductRecord);
                }
            }
            // Add the new products
            foreach (var productQuantity in lookupNew
                .Where(kvp => kvp.Value > 0)
                .Select(kvp => new ProductQuantity { ProductId = kvp.Key, Quantity = kvp.Value })) {

                AddProduct(productQuantity.Quantity, productQuantity.ProductId, record);
            }
        }
    }
}
