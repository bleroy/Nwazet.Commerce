using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization.Models;
using Orchard.Localization.Services;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.AdvancedSKUManagement")]
    public class LocalizationSKUUniquenessExceptionProvider : ISKUUniquenessExceptionProvider {

        private readonly ILocalizationService _localizationService;
        public LocalizationSKUUniquenessExceptionProvider(ILocalizationService localizationService) {
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
