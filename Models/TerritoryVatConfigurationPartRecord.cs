using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;
using System.Collections.Generic;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.AdvancedVAT")]
    public class TerritoryVatConfigurationPartRecord : ContentPartRecord {
        
        public TerritoryVatConfigurationPartRecord() {
            VatConfigurationIntersections = new List<TerritoryVatConfigurationIntersectionRecord>();
        }

        public virtual IList<TerritoryVatConfigurationIntersectionRecord> VatConfigurationIntersections { get; set; }
    }
}
