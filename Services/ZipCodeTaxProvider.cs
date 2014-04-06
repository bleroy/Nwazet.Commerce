using System.Collections.Generic;
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
            return _contentManager
                .Query<ZipCodeTaxPart>()
                .ForVersion(VersionOptions.Published)
                .List();
        }
    }
}