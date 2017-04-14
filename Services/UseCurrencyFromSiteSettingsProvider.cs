using Nwazet.Commerce.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.CurrencyProviderBySiteSetting")]
    public class UseCurrencyFromSiteSettingsProvider : ICurrencyProvider {

        private readonly IOrchardServices _orchardServices;
        public Localizer T { get; set; }

        public UseCurrencyFromSiteSettingsProvider(IOrchardServices orchardServices) {
            _orchardServices = orchardServices;

            T = NullLocalizer.Instance;
        }
        public string Name { get { return "UseCurrencyFromSiteSettingsProvider"; } }
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
        public string GetCurrencyFormat() {
            return Currency.GetCurrencyFormat(CurrencyCode);
        }

        public string GetPriceString(double price) {
            return price.ToString(GetCurrencyFormat()) + " " + GetCurrencySymbol();
        }
        public string GetPriceString(decimal price) {
            return price.ToString(GetCurrencyFormat()) + " " + GetCurrencySymbol();
        }
        public string GetPriceString(double? price) {
            if (price.HasValue) {
                return GetPriceString(price.Value);
            }
            return string.Empty;
        }
        public string GetPriceString(decimal? price) {
            if (price.HasValue) {
                return GetPriceString(price.Value);
            }
            return string.Empty;
        }
    }
}
