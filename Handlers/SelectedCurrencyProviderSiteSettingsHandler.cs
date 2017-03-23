using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement.Handlers;

namespace Nwazet.Commerce.Handlers {
    public class SelectedCurrencyProviderSiteSettingsHandler : ContentHandler {

        public SelectedCurrencyProviderSiteSettingsHandler() {
            Filters.Add(new ActivatingFilter<SelectedCurrencyProviderSiteSettingsPart>("Site"));
        }
    }
}
