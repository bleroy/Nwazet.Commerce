using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.Localization.Models;

namespace Nwazet.Commerce.Services {
    public class BundleProductLocalizationServices : IBundleProductLocalizationServices {
        private readonly IContentManager _contentManager;

        public BundleProductLocalizationServices(
            IContentManager contentManager) {

            _contentManager = contentManager;
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
    }
}
