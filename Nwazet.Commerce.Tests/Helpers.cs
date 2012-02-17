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
    }
}
