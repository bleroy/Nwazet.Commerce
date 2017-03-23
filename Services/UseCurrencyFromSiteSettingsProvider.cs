using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
using Orchard;
using Orchard.Localization;
using Orchard.ContentManagement;

namespace Nwazet.Commerce.Services {
    public class UseCurrencyFromSiteSettingsProvider : ISelectedCurrencyProvider {

        private readonly IOrchardServices _orchardServices;
        public Localizer T { get; set; }

        public UseCurrencyFromSiteSettingsProvider(IOrchardServices orchardServices) {
            _orchardServices = orchardServices;

            T = NullLocalizer.Instance;
        }
        public string Name { get { return "UseCurrencyFromSiteSettingsProvider"; } }
        public bool Active { get; set; }
        public string Description {
            get
            {
                return T("Always use the currency that has been set as default in the store settings.").Text;
            }
        }

        public string CurrencyCode
        {
            get { return _orchardServices.WorkContext.CurrentSite.As<ECommerceCurrencySiteSettingsPart>().CurrencyCode; }
            private set { }
        }

        public string GetCurrencyDescription() {
            return Currency.CurrencyCodes[CurrencyCode];
        }

        public string GetCurrencySymbol() {
            return Currency.GetCurrencySymbol(CurrencyCode);
        }
    }
}
