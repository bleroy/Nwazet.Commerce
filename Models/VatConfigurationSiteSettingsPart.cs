using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.AdvancedVAT")]
    public class VatConfigurationSiteSettingsPart : ContentPart {

        /// <summary>
        /// This value being 0 means no default has been selected. That is a configuration error
        /// for the teneat, that will be signaled with an error notification until fixed.
        /// </summary>
        public int DefaultVatConfigurationId {
            get { return this.Retrieve(p => p.DefaultVatConfigurationId); }
            set { this.Store(p => p.DefaultVatConfigurationId, value); }
        }

        /// <summary>
        /// This Id is allowed to be 0. That condition corresponds to not setting a default
        /// territory, hence we would not compute vat when displaying prices on the front-end
        /// </summary>
        public int DefaultTerritoryForVatId {
            get { return this.Retrieve(p => p.DefaultTerritoryForVatId); }
            set { this.Store(p => p.DefaultTerritoryForVatId, value); }
        }
    }
}
