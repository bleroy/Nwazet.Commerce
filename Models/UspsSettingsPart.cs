using Nwazet.Commerce.Helpers;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Usps.Shipping")]
    public class UspsSettingsPart : ContentPart {
        public string UserId {
            get { return this.Get<string>("UserId"); } 
            set { this.Set("UserId", value); }
        }

        public string OriginZip {
            get { return this.Get<string>("OriginZip"); }
            set { this.Set("OriginZip", value); }
        }

        public bool CommercialPrices {
            get { return this.Get<bool>("CommercialPrices"); }
            set { this.Set("CommercialPrices", value); }
        }

        public bool CommercialPlusPrices {
            get { return this.Get<bool>("CommercialPlusPrices"); }
            set { this.Set("CommercialPlusPrices", value); }
        }
    }
}
