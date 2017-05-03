using Nwazet.Commerce.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Handlers {
    [OrchardFeature("Nwazet.CurrencyProviderBySiteSetting")]
    public class ECommerceCurrencySiteSettingsPartHandler : ContentHandler {

        public ECommerceCurrencySiteSettingsPartHandler() {
            Filters.Add(new ActivatingFilter<ECommerceCurrencySiteSettingsPart>("Site"));
        }
    }
}
