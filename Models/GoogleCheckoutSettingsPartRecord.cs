using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Google.Checkout")]
    public class GoogleCheckoutSettingsPartRecord : ContentPartRecord {
        public virtual string MerchantId { get; set; }
        public virtual bool UseSandbox { get; set; }
        public virtual string Currency { get; set; }
        public virtual string WeightUnit { get; set; }
        public virtual string AnalyticsId { get; set; }
    }
}
