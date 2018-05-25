using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.AdvancedVAT")]
    public class ProductVatConfigurationPartRecord : ContentPartRecord {

        public virtual VatConfigurationPartRecord VatConfiguration { get; set; }
    }
}
