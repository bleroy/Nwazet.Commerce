using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.ViewModels {
    [OrchardFeature("Nwazet.AdvancedVAT")]
    public class VatConfigurationTerritorySummaryViewModel {

        public string Name { get; set; }
        public ContentItem Item { get; set; }
        public decimal Rate { get; set; }
    }
}
