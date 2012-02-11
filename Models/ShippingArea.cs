using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Shipping")]
    public class ShippingArea {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
