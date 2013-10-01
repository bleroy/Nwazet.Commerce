using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.Taxes")]
    public class StateOrCountryTaxProvider : ITaxProvider {
        private readonly IContentManager _contentManager;
        private Localizer T { get; set; }

        public StateOrCountryTaxProvider(IContentManager contentManager) {
            _contentManager = contentManager;
            T = NullLocalizer.Instance;
        }

        public string ContentTypeName {
            get { return "StateOrCountryTax"; }
        }

        public string Name {
            get { return T("State or Country Tax").Text; }
        }

        public IEnumerable<ITax> GetTaxes() {
            return _contentManager
                .Query<StateOrCountryTaxPart, StateOrCountryTaxPartRecord>()
                .ForVersion(VersionOptions.Published)
                .OrderByDescending(t => t.Priority)
                .List();
        }
    }
}