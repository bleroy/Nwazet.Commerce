using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Usps.Shipping")]
    public class UspsShippingMethodProvider : IShippingMethodProvider {
        private readonly IContentManager _contentManager;
        private Localizer T { get; set; }

        public UspsShippingMethodProvider(IContentManager contentManager) {
            _contentManager = contentManager;
            T = NullLocalizer.Instance;
        }

        public string ContentTypeName { get { return "UspsShippingMethod"; } }
        public string Name { get { return T("USPS Shipping Method").Text; } }

        public IEnumerable<IShippingMethod> GetShippingMethods() {
            return _contentManager.Query<UspsShippingMethodPart, UspsShippingMethodPartRecord>().ForVersion(VersionOptions.Published).List();
        }
    }
}
