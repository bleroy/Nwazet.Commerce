using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;
using System.Collections.Generic;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.AdvancedVAT")]
    public class HierarchyVatConfigurationPartRecord : ContentPartRecord {

        public HierarchyVatConfigurationPartRecord() {
            VatConfigurationIntersections = new List<HierarchyVatConfigurationIntersectionRecord>();
        }
        
        public virtual IList<HierarchyVatConfigurationIntersectionRecord> VatConfigurationIntersections { get; set; }
    }
}
