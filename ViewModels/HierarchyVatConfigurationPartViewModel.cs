using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.ViewModels {
    [OrchardFeature("Nwazet.AdvancedVAT")]
    public class HierarchyVatConfigurationPartViewModel {

        public VatConfigurationDetailViewModel[] AllVatConfigurations  { get; set; }

    }
    
}
