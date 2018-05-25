using Nwazet.Commerce.Models;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.BaseTaxImplementations")]
    public class ZipCodeTaxComputationHelper : ITaxComputationHelper {

        public decimal ComputeTax(ITax tax, TaxContext context) {
            if (tax == null || context == null) {
                return 0;
            }
            if (tax.GetType() == typeof(ZipCodeTaxPart)) {
                return tax.ComputeTax(context.ShoppingCartQuantityProducts,
                    context.CartSubTotal,
                    context.ShippingPrice,
                    context.Country,
                    context.ZipCode);
            }
            return 0;
        }
    }
}
