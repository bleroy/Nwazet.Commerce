using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.AdvancedVAT")]
    public class ProductVatConfigurationPart : ContentPart<ProductVatConfigurationPartRecord> {

        private readonly LazyField<ContentItem> _vatConfiguration =
            new LazyField<ContentItem>();

        public LazyField<ContentItem> VatConfigurationField {
            get { return _vatConfiguration; }
        }

        public ContentItem VatConfiguration {
            get { return _vatConfiguration.Value; }
        }

        public VatConfigurationPart VatConfigurationPart {
            get { return VatConfiguration?.As<VatConfigurationPart>(); }
        }

        public bool UseDefaultVatCategory {
            get { return Record.VatConfiguration == null; }
        }
    }
}
