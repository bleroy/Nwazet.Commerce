using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.CurrencyProviderBySiteSetting")]
    public class ECommerceCurrencySiteSettingsPart : ContentPart {
        [Required,MaxLength(3)]
        public string CurrencyCode
        {
            get {
                string cc = this.Retrieve(p => p.CurrencyCode);
                return string.IsNullOrWhiteSpace(cc) ? "USD" : cc; //default is USD
            }
            set { this.Store(p => p.CurrencyCode, value); }
        }
    }
}
