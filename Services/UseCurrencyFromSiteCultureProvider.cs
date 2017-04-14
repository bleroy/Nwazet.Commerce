using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard;
using Orchard.Localization;

namespace Nwazet.Commerce.Services {
    public class UseCurrencyFromSiteCultureProvider : ICurrencyProvider {

        private readonly IOrchardServices _orchardServices;
        public Localizer T { get; set; }

        public UseCurrencyFromSiteCultureProvider(IOrchardServices orchardServices) {
            _orchardServices = orchardServices;

            T = NullLocalizer.Instance;
        }

        public string Name { get { return "UseCurrencyFromSiteCultureProvider"; } }
        public string Description
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
        private CultureInfo CurrentCulture { get { return CultureInfo.GetCultureInfo(CultureCode); } }

        public string CurrencyCode
        {
            get { return new RegionInfo(CurrentCulture.LCID).ISOCurrencySymbol; }
        }

        public string GetCurrencyDescription() {
            return Currency.CurrencyCodes[CurrencyCode];
        }

        public string GetCurrencySymbol() {
            return CurrentCulture.NumberFormat.CurrencySymbol;
        }
        public string GetCurrencyFormat() {
            return "C";
        }

        public string GetPriceString(double price) {
            return T("{0:c}", price).Text;
        }
        public string GetPriceString(decimal price) {
            return T("{0:c}", price).Text;
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
