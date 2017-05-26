using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
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
            return _contentManager
               .Query<ProductPart, ProductPartVersionRecord>(VersionOptions.Latest)
               .List()
               .Where(p => !p.Has<BundlePart>() && ids.Contains(p.Id));
        }

        public List<ProductEntryAutocomplete> GetProducts(string searchtext, List<int> exclude) {
            if (exclude == null)
                exclude = new List<int>();
            return _contentManager
                .Query<ProductPart, ProductPartVersionRecord>(VersionOptions.Latest)
                .List()
                .Where(p => !p.Has<BundlePart>() && !(exclude.Contains(p.Id)) && (p.Sku.IndexOf(searchtext,StringComparison.OrdinalIgnoreCase)>=0 || p.ContentItem.As<TitlePart>().Title.IndexOf(searchtext, StringComparison.OrdinalIgnoreCase) >= 0)).Select(x => new ProductEntryAutocomplete {
                    ProductId = x.Id,
                    EditUrl = _url.ItemEditUrl(x),
                    Quantity = 1,
                    DisplayText = _contentManager.GetItemMetadata(x.ContentItem).DisplayText
                }).OrderBy(x=>x.DisplayText).ToList();
        }
    }
}