using Nwazet.Commerce.Models;
using Orchard.Environment.Extensions;
using System.Linq;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.AdvancedVAT")]
    public class VatTaxComputationHelper : ITaxComputationHelper {

        private readonly IVatConfigurationService _vatConfigurationService;

        public VatTaxComputationHelper(
            IVatConfigurationService vatConfigurationService) {

            _vatConfigurationService = vatConfigurationService;
        }

        public decimal ComputeTax(ITax tax, TaxContext context) {
            if (tax == null || context == null) {
                return 0;
            }

            if (tax.GetType() == typeof(VatConfigurationPart)) {
                // with the introduction of IProductPriceService in the ShoppingCartBase computations, 
                // we don't need to compute the VAT separately here, because that would only lead to
                // paying it twice, unless the system is configured to always display prices "before tax"

                var destination = context.DestinationTerritory;
                // if destination is null, we may still pass it to the methods from IVatConfigurationService,
                // that will treat it as the default destination.

                // In these computations we don't care about the ITax object anymore. Each product
                // has its own VAT category, which will be used in computing the taxes. The issue with
                // this approach would be that we would end up computing the same things over and over 
                // for each VAT categoy configured. That is why in VatConfigurationProvider.GetTaxes()
                // we cheat and only return the first VatConfigurationPart.
                // ERROR: we will computing VAT over and over, because the GetTaxes() method must return
                // all vat configurations, otherwise the TaxAdminController cannot do its job.

                // This method should compute taxes only if the subtotal does not include them already.
                // i.e. this is equivalent to saying that we compute taxes only when the site setting is
                // telling to display prices "before tax"
                if (_vatConfigurationService.GetDefaultDestination() != null) {
                    return 0;
                }

                // This method should return the total tax for all the products from the context.
                return context
                    .ShoppingCartQuantityProducts
                    .Sum(scqp => {
                        var rate = _vatConfigurationService.GetRate(scqp.Product, destination);
                        var vat = rate *
                            (scqp.Quantity * (scqp.Price)
                            + scqp.LinePriceAdjustment);

                        return vat;
                    });
            }

            return 0;
        }
    }
}
