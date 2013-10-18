using System.ComponentModel.DataAnnotations;
using Nwazet.Commerce.Helpers;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Stripe")]
    public class StripeSettingsPart : ContentPart {
        [Required]
        public string PublishableKey {
            get { return this.Retrieve<string>("PublishableKey"); }
            set { this.Store("PublishableKey", value); }
        }

        public string SecretKey {
            get { return this.Retrieve<string>("SecretKey"); }
            set { this.Store("SecretKey", value); }
        }

        public string Currency {
            get { return this.Retrieve<string>("Currency"); }
            set { this.Store("Currency", value); }
        }
    }
}
