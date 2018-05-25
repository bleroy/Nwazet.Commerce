using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.ViewModels {
    [OrchardFeature("Nwazet.AdvancedVAT")]
    public class TerritoryVatConfigurationPartViewModel {
        public VatConfigurationDetailViewModel[] AllVatConfigurations { get; set; }
    }
}
