using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.AdvancedVAT")]
    public class TerritoryVatConfigurationIntersectionRecord {

        public virtual int Id { get; set; } // Primary Key
        public virtual TerritoryVatConfigurationPartRecord Territory { get; set; }
        public virtual VatConfigurationPartRecord VatConfiguration { get; set; }
        public virtual decimal Rate { get; set; }
    }
}
