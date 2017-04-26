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
    public class ProductAttributeLocalizationServices : IProductAttributeLocalizationServices {

        private readonly IProductAttributeService _attributeService;
        private readonly ILocalizationService _localizationService;
        public ProductAttributeLocalizationServices(
            ILocalizationService localizationService,
            IProductAttributeService attributeService) {

            _localizationService = localizationService;
            _attributeService = attributeService;
        }

        private Func<ProductAttributePart, Tuple<int, int>> NewAttributeIdSelector(LocalizationPart locPart) {
            return pap => {
                var ci = pap.ContentItem;
                if (_localizationService.GetContentCulture(ci) == locPart.Culture.Culture) {
                    //this attribute is fine
                    return new Tuple<int, int>(ci.Id, ci.Id);
                }
                var localized = _localizationService.GetLocalizations(ci)
                    .FirstOrDefault(lp => lp.Culture == locPart.Culture);
                return localized == null ?
                    new Tuple<int, int>(ci.Id, -ci.Id) : //negative id where we found no localization
                    new Tuple<int, int>(ci.Id, localized.Id);
            };
        }

        public IEnumerable<Tuple<int, int>> GetLocalizationIdPairs(ProductAttributesPart attributesPart, LocalizationPart locPart) {
            return _attributeService.GetAttributes(attributesPart.AttributeIds)
                .Select(NewAttributeIdSelector(locPart));
        }

        private Func<ProductAttributePart, bool> WrongCulturePredicate(LocalizationPart locPart) {
            return atp => {
                var lP = atp.As<LocalizationPart>();
                return lP == null || //attribute has no LocalizationPart (this should never be the case)
                lP.Culture != locPart.Culture;
            };
        }
        public IEnumerable<ProductAttributePart> GetAttributesInTheWrongCulture(ProductAttributesPart attributesPart, LocalizationPart locPart) {
            return _attributeService.GetAttributes(attributesPart.AttributeIds)
                .Where(WrongCulturePredicate(locPart));
        }
    }
}
