using Nwazet.Commerce.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.CurrencyProviderBySiteSetting")]
    public class UseCurrencyFromSiteSettingsProvider : CurrencyProviderBase {

        private readonly IOrchardServices _orchardServices;
        public Localizer T { get; set; }

        public UseCurrencyFromSiteSettingsProvider(IOrchardServices orchardServices)
            : base(orchardServices) {
            _orchardServices = orchardServices;

            T = NullLocalizer.Instance;
        }
        public override string Name { get { return "UseCurrencyFromSiteSettingsProvider"; } }
        public override string Description
        {
            get
            {
                return T("Always use the currency that has been set as default in the store settings.").Text;
            }
        }

        protected override Currency SelectedCurrency
        {
            get
            {
                return Currency.Currencies[
                    _orchardServices.WorkContext.CurrentSite.As<ECommerceCurrencySiteSettingsPart>().CurrencyCode];
            }
        }

    }
}
