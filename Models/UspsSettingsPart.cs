using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Usps.Shipping")]
    public class UspsSettingsPart : ContentPart {
        public string UserId {
            get { return this.Retrieve(p => p.UserId); } 
            set { this.Store(p => p.UserId, value); }
        }

        public string OriginZip {
            get { return this.Retrieve(p => p.OriginZip); }
            set { this.Store(p => p.OriginZip, value); }
        }

        public bool CommercialPrices {
            get { return this.Retrieve(p => p.CommercialPrices); }
            set { this.Store(p => p.CommercialPrices, value); }
        }

        public bool CommercialPlusPrices {
            get { return this.Retrieve(p => p.CommercialPlusPrices); }
            set { this.Store(p => p.CommercialPlusPrices, value); }
        }
    }
}
