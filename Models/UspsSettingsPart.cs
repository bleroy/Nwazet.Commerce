using Nwazet.Commerce.Helpers;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Usps.Shipping")]
    public class UspsSettingsPart : ContentPart {
        public string UserId {
            get { return this.Retrieve<string>("UserId"); } 
            set { this.Store("UserId", value); }
        }

        public string OriginZip {
            get { return this.Retrieve<string>("OriginZip"); }
            set { this.Store("OriginZip", value); }
        }

        public bool CommercialPrices {
            get { return this.Retrieve<bool>("CommercialPrices"); }
            set { this.Store("CommercialPrices", value); }
        }

        public bool CommercialPlusPrices {
            get { return this.Retrieve<bool>("CommercialPlusPrices"); }
            set { this.Store("CommercialPlusPrices", value); }
        }
    }
}
