using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.Shipping")]
    public class SizeBasedShippingMethodProvider : IShippingMethodProvider {
        private readonly IContentManager _contentManager;
        private Localizer T { get; set; }

        public SizeBasedShippingMethodProvider(IContentManager contentManager) {
            T = NullLocalizer.Instance;
            _contentManager = contentManager;
        }

        public string ContentTypeName { get { return "SizeBasedShippingMethod"; } }
        public string Name { get { return T("Size-Based Shipping Method").Text; } }

        public IEnumerable<IShippingMethod> GetShippingMethods() {
            return _contentManager.Query<SizeBasedShippingMethodPart, SizeBasedShippingMethodPartRecord>().ForVersion(VersionOptions.Published).List();
        }
    }
}
