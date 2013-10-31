using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Stripe")]
    public class StripeSettingsPart : ContentPart {
        [Required]
        public string PublishableKey {
            get { return Retrieve<string>("PublishableKey"); }
            set { Store("PublishableKey", value); }
        }

        public string SecretKey {
            get { return Retrieve<string>("SecretKey"); }
            set { Store("SecretKey", value); }
        }

        public string Currency {
            get { return Retrieve<string>("Currency"); }
            set { Store("Currency", value); }
        }
    }
}
