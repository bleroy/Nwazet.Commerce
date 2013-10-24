using System;
using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.Tests.Stubs;
using Orchard;

namespace Nwazet.Commerce.Tests.Helpers {
    public class ShippingHelpers {
        public static WeightBasedShippingMethodPart BuildWeightBasedShippingMethod(
            double price,
            double minimumWeight = 0,
            double maximumWeight = double.PositiveInfinity) {

            var result = new WeightBasedShippingMethodPart();
            ContentHelpers.PreparePart<WeightBasedShippingMethodPart, WeightBasedShippingMethodPartRecord>(result,
                "WeightBasedShippingMethod", 0);
            result.Price = price;
            result.MinimumWeight = minimumWeight;
            result.MaximumWeight = maximumWeight;
            return result;
        }

        public static SizeBasedShippingMethodPart BuildSizeBasedShippingMethod(
            double price,
            string size = null,
            int priority = 0) {

            var result = new SizeBasedShippingMethodPart();
            ContentHelpers.PreparePart<SizeBasedShippingMethodPart, SizeBasedShippingMethodPartRecord>(result,
                "SizeBasedShippingMethod", 0);
            result.Price = price;
            result.Size = size;
            result.Priority = priority;
            return result;
        }

        public static UspsShippingMethodPart BuildUspsShippingMethod(
            string size = null,
            int priority = 0) {

            var result = new UspsShippingMethodPart();
            ContentHelpers.PreparePart<UspsShippingMethodPart, UspsShippingMethodPartRecord>(result,
                "UspsShippingMethod", 0);
            result.Size = size;
            result.Priority = priority;
            return result;
        }

        public static IWorkContextAccessor GetUspsWorkContextAccessor(string originZip,
            bool commercialPrices,
            bool commercialPlusPrices, double price = 10) {
            return new WorkContextAccessorStub(new Dictionary<Type, object> {
                {
                    typeof (IUspsService),
                    new UspsServiceStub("", originZip, commercialPrices, commercialPlusPrices, price)
                }
            });
        }
    }
}