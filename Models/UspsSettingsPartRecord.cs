using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Usps.Shipping")]
    public class UspsSettingsPartRecord : ContentPartRecord
    {
        public virtual string UserId { get; set; }
        public virtual string OriginZip { get; set; }
        public virtual bool CommercialPrices { get; set; }
        public virtual bool CommercialPlusPrices { get; set; }
    }
}
