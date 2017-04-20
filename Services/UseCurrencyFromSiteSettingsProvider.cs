using Nwazet.Commerce.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.CurrencyProviderBySiteSetting")]
    public class UseCurrencyFromSiteSettingsProvider : CurrencyProviderBase {
        private readonly IWorkContextAccessor _workContextAccessor;
        public Localizer T { get; set; }

        public UseCurrencyFromSiteSettingsProvider(IWorkContextAccessor workContextAccessor)
            : base(workContextAccessor) {
            _workContextAccessor = workContextAccessor;

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
                    _workContextAccessor.GetContext().CurrentSite.As<ECommerceCurrencySiteSettingsPart>().CurrencyCode];
            }
        }

    }
}
