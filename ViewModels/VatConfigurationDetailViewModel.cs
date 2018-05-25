using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.ViewModels {
    [OrchardFeature("Nwazet.AdvancedVAT")]
    public class VatConfigurationDetailViewModel {

        public int VatConfigurationPartId { get; set; }
        public string VatConfigurationPartText { get; set; }
        public bool IsSelected { get; set; }
        public decimal? Rate { get; set; }
        public string RateString { get; set; }
    }
}
