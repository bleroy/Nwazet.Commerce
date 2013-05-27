using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Usps.Shipping")]
    public class UspsSettingsPart : ContentPart<UspsSettingsPartRecord> {
        public string UserId { get { return Record.UserId; } set { Record.UserId = value; } }
        public string OriginZip { get { return Record.OriginZip; } set { Record.OriginZip = value; } }
        public bool CommercialPrices { get { return Record.CommercialPrices; } set { Record.CommercialPrices = value; } }
        public bool CommercialPlusPrices { get { return Record.CommercialPlusPrices; } set { Record.CommercialPlusPrices = value; } }
    }
}
