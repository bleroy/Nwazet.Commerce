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
                return this.Retrieve(p => p.CurrencyCode);
            }
            set { this.Store(p => p.CurrencyCode, value); }
        }
    }
}
