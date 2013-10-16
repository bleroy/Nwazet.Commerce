using System.ComponentModel.DataAnnotations;
using Nwazet.Commerce.Helpers;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Stripe")]
    public class StripeSettingsPart : ContentPart {
        [Required]
        public string PublishableKey {
            get { return this.Get<string>("PublishableKey"); }
            set { this.Set("PublishableKey", value); }
        }

        public string SecretKey {
            get { return this.Get<string>("SecretKey"); }
            set { this.Set("SecretKey", value); }
        }

        public string Currency {
            get { return this.Get<string>("Currency"); }
            set { this.Set("Currency", value); }
        }
    }
}
