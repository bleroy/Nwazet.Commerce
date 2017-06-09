using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nwazet.Commerce.Extensions;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.ViewModels;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Mvc.Html;

namespace Nwazet.Commerce.Services {
    public class BundleAutocompleteService : IBundleAutocompleteService {

        private readonly IContentManager _contentManager;
        private readonly UrlHelper _url;

        public BundleAutocompleteService(
            IContentManager contentManager,
            UrlHelper url) {
            _contentManager = contentManager;
            _url = url;
        }
        protected virtual bool ConsiderProductValid(IContent prod, BundlePart part) {
            return true;
        }
        public BundleViewModel BuildEditorViewModel(BundlePart part) {
            var bundleProductQuantities = part.ProductQuantities.ToDictionary(pq => pq.ProductId, pq => pq.Quantity);
            var ids = part.ProductQuantities.Select(x => x.ProductId);
            return new BundleViewModel {
                Products = GetProductParts(ids)
                    .Where(p => ConsiderProductValid(p, part))
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

        private IEnumerable<ProductPart> GetProductParts(IEnumerable<int> ids) {
            return _contentManager.GetMany<ProductPart>(ids, VersionOptions.Latest, QueryHints.Empty)
                    .Where(p => !p.Has<BundlePart>());
        }

        public List<ProductEntryAutocomplete> GetProducts(string searchText, List<int> excludedProductIds) {
            if (excludedProductIds == null)
                excludedProductIds = new List<int>();
            var productAlias = "productPartVersionRecord";
            var titleAlias = "titlePartRecord";
            Action<IAliasFactory> productPartRecordAlias = x => x.ContentPartRecord<ProductPartVersionRecord>().Named(productAlias);
            Action<IAliasFactory> titlePartRecordAlias = x => x.ContentPartRecord<TitlePartRecord>().Named(titleAlias);
            Action<IHqlExpressionFactory> titleSearch = title => title.InsensitiveLikeSpecificAlias(titleAlias, "Title", searchText, HqlMatchMode.Anywhere);
            Action<IHqlExpressionFactory> skuSearch = sku => sku.InsensitiveLikeSpecificAlias(productAlias, "Sku", searchText, HqlMatchMode.Anywhere);
            Action<IHqlExpressionFactory> idNotExcluded = p => p.Gt("Id", 0);
            if (excludedProductIds.Count > 0)
                idNotExcluded = p => p.Not(q => q.In("Id", excludedProductIds.ToArray()));
            return _contentManager.HqlQuery()
                .ForVersion(VersionOptions.Latest)
                .Join(productPartRecordAlias)
                .Join(titlePartRecordAlias)
                .Where(a => a.ContentItem(),
                    x => x.And(
                      idNotExcluded,
                           search => search.Or(
                           titleSearch,
                           skuSearch)))
               .OrderBy(titlePartRecordAlias, o => o.Asc("Title"))
               .List()
               .Select(x => new ProductEntryAutocomplete {
                   ProductId = x.Id,
                   EditUrl = _url.ItemEditUrl(x),
                   Quantity = 1,
                   DisplayText = _contentManager.GetItemMetadata(x).DisplayText
               })
               .ToList();
        }
    }
}