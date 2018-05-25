using System.Collections.Generic;
using System.Linq;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization.Models;
using Orchard.Localization.Services;
using Orchard;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.AdvancedSKUManagement")]
    public class LocalizationSKUUniquenessHelper : ISKUUniquenessHelper {

        private ILocalizationService _localizationService;
        private readonly IWorkContextAccessor _workContextAccessor;
        public LocalizationSKUUniquenessHelper(
            IWorkContextAccessor workContextAccessor) {

            _workContextAccessor = workContextAccessor;
        }

        public IEnumerable<int> GetIdsOfValidSKUDuplicates(ProductPart part) {

            LocalizationPart lPart = part.ContentItem.As<LocalizationPart>();
            if (lPart == null) {
                // the ContentItem is not Localizable so this does not apply
                return new int[0];
            }

            if (_workContextAccessor.GetContext().TryResolve<ILocalizationService>(out _localizationService)) {
                return _localizationService
                    .GetLocalizations(part.ContentItem, VersionOptions.Latest)
                    .Select(ci => ci.Id);
            }
            // there is no localization service, so this does not apply
            return new int[0];
        }
    }
}
