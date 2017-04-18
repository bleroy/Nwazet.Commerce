using System.Globalization;
using Orchard;
using Orchard.Localization;

namespace Nwazet.Commerce.Services {
    public class UseCurrencyFromSiteCultureProvider : CurrencyProviderBase {

        private readonly IOrchardServices _orchardServices;
        public Localizer T { get; set; }

        public UseCurrencyFromSiteCultureProvider(IOrchardServices orchardServices)
            : base(orchardServices) {
            _orchardServices = orchardServices;

            T = NullLocalizer.Instance;
        }

        public override string Name { get { return "UseCurrencyFromSiteCultureProvider"; } }
        public override string Description
        {
            get
            {
                return T("Use the currency of the site's culture.").Text;
            }
        }

        private string CultureCode
        {
            get { return _orchardServices.WorkContext.CurrentSite.SiteCulture; }
        }
        protected override Currency SelectedCurrency
        {
            get
            {
                return Currency.Currencies[
                    new RegionInfo(
                        CultureInfo.GetCultureInfo(CultureCode).LCID)
                        .ISOCurrencySymbol];
            }
        }

    }
}
