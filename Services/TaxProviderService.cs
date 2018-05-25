using Nwazet.Commerce.Models;
using Orchard.Environment.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.Taxes")]
    public class TaxProviderService : ITaxProviderService {

        private readonly IEnumerable<ITaxComputationHelper> _taxComputationHelpers;
        private readonly ITerritoriesRepositoryService _territoriesRepositoryService;

        public TaxProviderService(
            IEnumerable<ITaxComputationHelper> taxComputationHelpers,
            ITerritoriesRepositoryService territoriesRepositoryService) {

            _taxComputationHelpers = taxComputationHelpers;
            _territoriesRepositoryService = territoriesRepositoryService;
        }

        public TaxContext CreateContext(
            IEnumerable<ShoppingCartQuantityProduct> productQuantities,
            decimal subtotal,
            decimal shippingCost,
            string country, string zipCode) {

            return new TaxContext() {
                ShoppingCartQuantityProducts = productQuantities,
                CartSubTotal = subtotal,
                ShippingPrice = shippingCost,
                Country = country,
                ZipCode = zipCode,
                DestinationTerritory = FindDestination(country, zipCode)
            };
        }

        public TaxContext CreateContext(
            IEnumerable<ShoppingCartQuantityProduct> productQuantities,
            decimal subtotal,
            decimal shippingCost,
            string country, string zipCode,
            TerritoryInternalRecord destinationTerritory) {

            return new TaxContext() {
                ShoppingCartQuantityProducts = productQuantities,
                CartSubTotal = subtotal,
                ShippingPrice = shippingCost,
                Country = country,
                ZipCode = zipCode,
                DestinationTerritory = destinationTerritory
            };
        }

        private TerritoryInternalRecord FindDestination(string country, string zipCode) {
            if (string.IsNullOrWhiteSpace(country) && string.IsNullOrWhiteSpace(zipCode)) {
                return null;
            }
            var destination = !string.IsNullOrWhiteSpace(zipCode)
                ? _territoriesRepositoryService.GetTerritoryInternal(zipCode)
                : null;
            if (destination == null) {
                destination = !string.IsNullOrWhiteSpace(country)
                    ? _territoriesRepositoryService.GetTerritoryInternal(country)
                    : null;
            }
            return destination;
        }

        public decimal TotalTaxes(ITax tax, TaxContext context) {
            return _taxComputationHelpers.Sum(tch => tch.ComputeTax(tax, context));
        }

        public IEnumerable<decimal> ItemizedTaxes(ITax tax, TaxContext context) {
            foreach (var productQuantity in context.ShoppingCartQuantityProducts) {
                var singleItemContext = CreateContext(
                    new ShoppingCartQuantityProduct[] { productQuantity },
                    productQuantity.Price * productQuantity.Quantity + productQuantity.LinePriceAdjustment,
                    0,
                    context.Country,
                    context.ZipCode,
                    context.DestinationTerritory);
                yield return _taxComputationHelpers.Sum(tch => tch.ComputeTax(tax, singleItemContext));
            }
        }

        public decimal ShippingTaxes(ITax tax, TaxContext context) {
            var shippingContext = CreateContext(
                null,
                0,
                context.ShippingPrice,
                context.Country,
                context.ZipCode
                );
            return _taxComputationHelpers.Sum(tch => tch.ComputeTax(tax, shippingContext));
        }
    }
}
