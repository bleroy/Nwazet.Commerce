using System.Collections.Generic;
using System.Linq;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization.Models;
using Orchard.Localization.Services;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.AdvancedSKUManagement")]
    public class LocalizationSKUUniquenessHelper : ISKUUniquenessHelper {

        private readonly ILocalizationService _localizationService;
        public LocalizationSKUUniquenessHelper(ILocalizationService localizationService) {
            _localizationService = localizationService;
        }

        public IEnumerable<int> GetIdsOfValidSKUDuplicates(ProductPart part) {
            LocalizationPart lPart = part.ContentItem.As<LocalizationPart>();
            if (lPart == null) {
                //the ContentItem is not Localizabel so this does not apply
                return new int[0];
            }
            return _localizationService.GetLocalizations(part.ContentItem, VersionOptions.Latest).Select(ci => ci.Id);
        }
    }
}
