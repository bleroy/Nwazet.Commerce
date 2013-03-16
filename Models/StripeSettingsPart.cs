using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Stripe")]
    public class StripeSettingsPart : ContentPart<StripeSettingsPartRecord> {
        [Required]
        public string PublishableKey { get { return Record.PublishableKey; } set { Record.PublishableKey = value; } }
        public string SecretKey { get { return Record.SecretKey; } set { Record.SecretKey = value; } }
        public string Currency { get { return Record.Currency; } set { Record.Currency = value; } }
    }
}
