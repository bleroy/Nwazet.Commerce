using System;

namespace Nwazet.Commerce.Models {
    public static class ConversionUtilities {
        public static double? ToDouble(this String ds) {
            double o;
            double? r = null;

            if (double.TryParse(ds, out o)) {
                r = o;
            };
            return r;
        }
    }
}
