using System.Collections.Generic;
using System.Linq;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.Taxes")]
    public class ZipCodeTaxProvider : ITaxProvider {
        private readonly IContentManager _contentManager;
        private Localizer T { get; set; }

        public ZipCodeTaxProvider(IContentManager contentManager) {
            _contentManager = contentManager;
            T = NullLocalizer.Instance;
        }

        public string ContentTypeName {
            get { return "ZipCodeTax"; }
        }

        public string Name {
            get { return T("US Zip Code Tax").Text; }
        }

        public IEnumerable<ITax> GetTaxes() {
            var taxTypes = _contentManager
                .GetContentTypeDefinitions()
                .Where(d => d.Parts.Any(p => p.PartDefinition.Name == "ZipCodeTaxPart"))
                .Select(d => d.Name)
                .ToArray();
            return _contentManager
                .Query<ZipCodeTaxPart>(taxTypes)
                .ForVersion(VersionOptions.Published)
                .List();
        }
    }
}