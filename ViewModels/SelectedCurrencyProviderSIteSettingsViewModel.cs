using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Services;

namespace Nwazet.Commerce.ViewModels {
    public class SelectedCurrencyProviderSiteSettingsViewModel {
        public List<ISelectedCurrencyProvider> Providers { get; set; }

        public SelectedCurrencyProviderSiteSettingsViewModel() {
            Providers = new List<ISelectedCurrencyProvider>();
        }
    }
}
