using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.Localization.Models;
using Orchard.Localization.Services;

namespace Nwazet.Commerce.Services {
    public class BundleProductLocalizationServices : IBundleProductLocalizationServices {
        private readonly IContentManager _contentManager;
        private readonly ILocalizationService _localizationService;

        public BundleProductLocalizationServices(
            IContentManager contentManager,
            ILocalizationService localizationService) {

            _contentManager = contentManager;
            _localizationService = localizationService;
        }

        public bool HasDifferentCulture(IContent ci, LocalizationPart locPart) {
            var lP = ci.As<LocalizationPart>();
            return lP != null && //has a LocalizationPart AND
                (lP.Culture == null || //culture undefined OR
                    (string.IsNullOrWhiteSpace(lP.Culture.Culture) || //culture undefined OR
                        (lP.Culture != locPart.Culture))); //culture different than the product's 
        }

        public bool ValidLocalizationPart(LocalizationPart part) {
            return part != null &&
                part.Culture != null &&
                !string.IsNullOrWhiteSpace(part.Culture.Culture);
        }

        private Func<IContent, bool> WrongCulturePredicate(LocalizationPart locPart) {
            return atp => {
                var lP = atp.As<LocalizationPart>();
                return lP == null || //attribute has no LocalizationPart (this should never be the case)
                lP.Culture != locPart.Culture;
            };
        }
        public IEnumerable<IContent> GetProductsInTheWrongCulture(BundlePart bundlePart, LocalizationPart locPart) {
            return bundlePart.ProductIds
                .Select(pid => _contentManager.Get(pid))
                .Where(WrongCulturePredicate(locPart));
        }
        public IEnumerable<IContent> GetProductsInTheWrongCulture(IEnumerable<int> productIds, LocalizationPart locPart) {
            return productIds
                .Select(id => _contentManager.Get(id))
                .Where(WrongCulturePredicate(locPart));
        }

        private Func<ProductQuantity, ProductQuantityPair> TranslatedProductSelector(LocalizationPart locPart) {
            return pq => {
                var ci = _contentManager.Get(pq.ProductId);
                if (_localizationService.GetContentCulture(ci) == locPart.Culture.Culture) {
                    //this product is fine
                    return new ProductQuantityPair(pq, pq.ProductId);
                }
                var localized = _localizationService.GetLocalizations(ci)
                .FirstOrDefault(lp => lp.Culture == locPart.Culture);
                if (localized == null) { //found no localization
                    return new ProductQuantityPair(pq, -pq.ProductId);
                }
                return new ProductQuantityPair(pq, localized.Id);
            };
        }

        public IEnumerable<ProductQuantityPair> GetLocalizationIdPairs(BundlePart bundlePart, LocalizationPart locPart) {
            return bundlePart.ProductQuantities
                .Select(TranslatedProductSelector(locPart));
        }

        public IEnumerable<ProductQuantityPair> GetLocalizationIdPairs(IEnumerable<ProductQuantity> originalProducts, LocalizationPart locPart) {
            return originalProducts.Select(TranslatedProductSelector(locPart));
        }
    }
}
