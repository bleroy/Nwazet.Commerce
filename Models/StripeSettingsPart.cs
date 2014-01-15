using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Stripe")]
    public class StripeSettingsPart : ContentPart {
        [Required]
        public string PublishableKey {
            get { return this.Retrieve(p => p.PublishableKey); }
            set { this.Store(p => p.PublishableKey, value); }
        }

        public string SecretKey {
            get { return this.Retrieve(p => p.SecretKey); }
            set { this.Store(p => p.SecretKey, value); }
        }

        public string Currency {
            get { return this.Retrieve(p => p.Currency); }
            set { this.Store(p => p.Currency, value); }
        }
    }
}
