using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using Orchard.Localization;

namespace Nwazet.Commerce.Handlers {
    [OrchardFeature("Nwazet.TieredPricing")]
    public class ProductSettingsPartHandler : ContentHandler {
        public ProductSettingsPartHandler() {
            T = NullLocalizer.Instance;
            Filters.Add(new ActivatingFilter<ProductSettingsPart>("Site"));
        }

        public Localizer T { get; set; }
    }
}