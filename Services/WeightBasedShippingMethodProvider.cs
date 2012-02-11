using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.Shipping")]
    public class WeightBasedShippingMethodProvider : IShippingMethodProvider {
        private readonly IContentManager _contentManager;
        private Localizer T { get; set; }

        public WeightBasedShippingMethodProvider(IContentManager contentManager) {
            T = NullLocalizer.Instance;
            _contentManager = contentManager;
        }

        public string ContentTypeName { get { return "WeightBasedShippingMethod"; } }
        public string Name { get { return T("Weight-Based Shipping Method").Text; } }

        public IEnumerable<IShippingMethod> GetShippingMethods() {
            return _contentManager.Query<WeightBasedShippingMethodPart, WeightBasedShippingMethodPartRecord>().ForVersion(VersionOptions.Published).List();
        }
    }
}
