using Nwazet.Commerce.Models;

namespace Nwazet.Commerce.Tests {
    public class Helpers {
        public static WeightBasedShippingMethodPart BuildWeightBasedShippingMethod(
            double price,
            double minimumWeight = 0,
            double maximumWeight = double.PositiveInfinity
            ) {

            var result = new WeightBasedShippingMethodPart {
                Record = new WeightBasedShippingMethodPartRecord(),
                Price = price,
                MinimumWeight = minimumWeight,
                MaximumWeight = maximumWeight
            };
            return result;
        }

        public static SizeBasedShippingMethodPart BuildSizeBasedShippingMethod(
            double price,
            string size = null,
            int priority = 0) {
            return new SizeBasedShippingMethodPart {
                Record = new SizeBasedShippingMethodPartRecord(),
                Price = price,
                Size = size,
                Priority = priority
            };
        }
    }
}
