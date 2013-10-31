using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Usps.Shipping")]
    public class UspsSettingsPart : ContentPart {
        public string UserId {
            get { return Retrieve<string>("UserId"); } 
            set { Store("UserId", value); }
        }

        public string OriginZip {
            get { return Retrieve<string>("OriginZip"); }
            set { Store("OriginZip", value); }
        }

        public bool CommercialPrices {
            get { return Retrieve<bool>("CommercialPrices"); }
            set { Store("CommercialPrices", value); }
        }

        public bool CommercialPlusPrices {
            get { return Retrieve<bool>("CommercialPlusPrices"); }
            set { Store("CommercialPlusPrices", value); }
        }
    }
}
