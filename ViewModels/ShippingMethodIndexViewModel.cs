using System.Collections.Generic;
using Nwazet.Commerce.Services;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.ViewModels {
    [OrchardFeature("Nwazet.Shipping")]
    public class ShippingMethodIndexViewModel {
        public IList<IShippingMethodProvider> ShippingMethodProviders { get; set; }
        public IList<IShippingMethod> ShippingMethods { get; set; }
        public dynamic Pager { get; set; }
    }
}
