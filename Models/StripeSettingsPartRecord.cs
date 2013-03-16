using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Stripe")]
    public class StripeSettingsPartRecord : ContentPartRecord {
        public virtual string PublishableKey { get; set; }
        public virtual string SecretKey { get; set; }
        public virtual string Currency { get; set; }
    }
}
