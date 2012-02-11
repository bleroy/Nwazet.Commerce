using Nwazet.Commerce.Services;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Shipping")]
    public class ShippingMethodAndCost {
        public IShippingMethod ShippingMethod { get; set; }
        public double Price { get; set; }
    }
}
