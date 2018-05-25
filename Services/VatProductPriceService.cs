using Nwazet.Commerce.Models;
using Orchard;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.AdvancedVAT")]
    public class VatProductPriceService : BaseProductPriceService {

        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IVatConfigurationService _vatConfigurationService;
        private readonly ITerritoriesRepositoryService _territoriesRepositoryService;
        private readonly ITerritoriesService _territoriesService;

        public VatProductPriceService(
            IWorkContextAccessor workContextAccessor,
            IVatConfigurationService vatConfigurationService,
            ITerritoriesRepositoryService territoriesRepositoryService,
            ITerritoriesService territoriesService) {

            _workContextAccessor = workContextAccessor;
            _vatConfigurationService = vatConfigurationService;
            _territoriesRepositoryService = territoriesRepositoryService;
            _territoriesService = territoriesService;
        }
        
        public override decimal GetDiscountPrice(ProductPart part) {
            return GetPrice(part, base.GetDiscountPrice(part));
        }

        public override decimal GetDiscountPrice(ProductPart part, string country, string zipCode) {
            return GetPrice(part, base.GetDiscountPrice(part), country, zipCode);
        }

        public override decimal GetPrice(ProductPart part) {
            return GetPrice(part, base.GetPrice(part));
        }

        public override decimal GetPrice(ProductPart part, decimal basePrice) {
            return basePrice + basePrice * GetRate(part);
        }

        public override decimal GetPrice(ProductPart part, string country, string zipCode) {
            return GetPrice(part, base.GetPrice(part), country, zipCode);
        }

        public override decimal GetPrice(ProductPart part, decimal basePrice, string country, string zipCode) {
            return basePrice + basePrice * GetRate(part, FindDestination(country, zipCode));
        }

        private decimal GetRate(ProductPart part) {
            return _vatConfigurationService.GetRate(part);
        }

        private decimal GetRate(ProductPart part, TerritoryInternalRecord destination) {
            return _vatConfigurationService.GetRate(part, destination);
        }

        private TerritoryInternalRecord FindDestination(string country, string zipCode) {
            if (_vatConfigurationService.GetDefaultDestination() == null) {
                // the configuration is telling that the prices on the frontend should be 
                // "before tax"
                return null;
            }

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
    }
}
