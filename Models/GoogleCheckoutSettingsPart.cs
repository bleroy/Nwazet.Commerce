using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Google.Checkout")]
    public class GoogleCheckoutSettingsPart : ContentPart<GoogleCheckoutSettingsPartRecord> {
        [Required]
        public string MerchantId { get { return Record.MerchantId; } set { Record.MerchantId = value; } }
        public bool UseSandbox { get { return Record.UseSandbox; } set { Record.UseSandbox = value; } }
        [DefaultValue("USD")]
        public string Currency { get { return Record.Currency; } set { Record.Currency = value; } }
        [DefaultValue("LB")]
        public string WeightUnit { get { return Record.WeightUnit; } set { Record.WeightUnit = value; } }
        public string AnalyticsId { get { return Record.AnalyticsId; } set { Record.AnalyticsId = value; } }
    }
}
