using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Tokens;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.AdvancedSKUManagement")]
    public class SKUGenerationServices : ISKUGenerationServices {

        private readonly IOrchardServices _orchardServices;
        private readonly ITokenizer _tokenizer;
        private readonly IEnumerable<ISKUUniquenessExceptionProvider> _SKUUniquenessExceptionProviders;
        private readonly IContentManager _contentManager;

        public string DefaultSkuPattern { get { return "SKU-{Content.Slug}"; } }

        public SKUGenerationServices(IOrchardServices orchardServices,
            ITokenizer tokenizer,
            IEnumerable<ISKUUniquenessExceptionProvider> SKUUniquenessExceptionProviders,
            IContentManager contentManager) {

            _orchardServices = orchardServices;
            _tokenizer = tokenizer;
            _SKUUniquenessExceptionProviders = SKUUniquenessExceptionProviders;
            _contentManager = contentManager;
        }
        private AdvancedSKUsSiteSettingPart Settings { get; set; }
        public AdvancedSKUsSiteSettingPart GetSettings() {
            if (Settings == null) {
                Settings = _orchardServices.WorkContext.CurrentSite.As<AdvancedSKUsSiteSettingPart>();
            }
            return Settings;
        }

        private IDictionary<string, object> BuildTokenContext(IContent item) {
            return new Dictionary<string, object> { { "Content", item } };
        }

        public string GenerateSku(ProductPart part) {
            if (part == null) {
                throw new ArgumentNullException("part");
            }
            var pattern = GetSettings().SKUPattern;
            if (Settings.AllowCustomPattern && !string.IsNullOrWhiteSpace(part.Sku)) {
                pattern = part.Sku;
            }
            if (string.IsNullOrWhiteSpace(pattern)) {
                //use a default pattern
                pattern = DefaultSkuPattern;
            }
            var sku = _tokenizer.Replace(pattern, 
                BuildTokenContext(part.ContentItem), 
                new ReplaceOptions { Encoding = ReplaceOptions.NoEncode });

            return sku;
        }

        private static int? GetSkuVersion(string sku, string conflictingSku) {
            int v;
            var skuParts = conflictingSku.Split(new[] { sku }, StringSplitOptions.RemoveEmptyEntries);

            if (skuParts.Length == 0) {
                return 2;
            }

            return Int32.TryParse(skuParts[0].TrimStart('-'), out v) ?
                (int?)++v :
                null;
        }

        private string GenerateUniqueSku(ProductPart part, IEnumerable<string> existingSkus) {
            if (existingSkus == null || !existingSkus.Contains(part.Sku)) {
                return part.Sku;
            }

            int? version = existingSkus
                .Select(s => GetSkuVersion(part.Sku, s))
                .OrderBy(i => i).LastOrDefault();

            return version != null ?
                string.Format("{0}-{!}", part.Sku, version) :
                part.Sku;
        }

        private IEnumerable<ProductPart> GetSimilarSkus(string sku) {
            return _contentManager.Query<ProductPart, ProductPartRecord>()
                .Where(part => part.Sku != null && part.Sku.StartsWith(sku))
                .List();
        }

        public bool ProcessSku(ProductPart part) {
            var similarSkuParts = GetSimilarSkus(part.Sku);
            //dont include the part we are processing
            similarSkuParts = similarSkuParts.Where(pp => pp.ContentItem.Id != part.ContentItem.Id);
            //consider exceptions to uniqueness
            if (_SKUUniquenessExceptionProviders.Any()) {
                var exceptionIds = _SKUUniquenessExceptionProviders.SelectMany(p => p.GetIdsOfValidSKUDuplicates(part));
                similarSkuParts = similarSkuParts.Where(pp => !exceptionIds.Contains(pp.Id));
            }

            if (similarSkuParts.Any()) {
                var oldSku = part.Sku;
                part.Sku = GenerateUniqueSku(part, similarSkuParts.Select(p => p.Sku));

                return part.Sku == oldSku;
            }

            return true;
        }
    }
}
