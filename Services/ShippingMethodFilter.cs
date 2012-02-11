using System.Linq;
using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.Shipping")]
    public class ShippingMethodFilter {
        public static IEnumerable<ShippingMethodAndCost> Filter(
            IEnumerable<IShippingMethod> shippingMethods,
            IEnumerable<ShoppingCartQuantityProduct> productQuantities) {

            return shippingMethods
                .Select(
                    method => new ShippingMethodAndCost {
                        Price = method.ComputePrice(productQuantities),
                        ShippingMethod = method
                    })
                    .Where(mc => mc.Price.CompareTo(0) >= 0);
        }
    }
}
