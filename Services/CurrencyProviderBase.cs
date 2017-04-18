using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard;

namespace Nwazet.Commerce.Services {
    public abstract class CurrencyProviderBase : ICurrencyProvider {
        private readonly IOrchardServices _orchardServices;

        public CurrencyProviderBase(
            IOrchardServices orchardServices) {

            _orchardServices = orchardServices;
        }

        public virtual string Name { get { return ""; } }
        public virtual string Description { get { return ""; } }

        protected virtual Currency SelectedCurrency { get; }

        public string CurrencyCode { get { return SelectedCurrency.CurrencyCode; } }

        public string GetCurrencyDescription() {
            return SelectedCurrency.CurrencyName;
        }

        public string GetCurrencySymbol() {
            return SelectedCurrency.Symbol;
        }

        public string GetPriceString(double? price) {
            if (price.HasValue) {
                return SelectedCurrency.PriceAsString(price.Value,
                        _orchardServices.WorkContext.CurrentCulture
                    );
            }
            return string.Empty;
        }
        public string GetPriceString(decimal? price) {
            if (price.HasValue) {
                return SelectedCurrency.PriceAsString(price.Value,
                        _orchardServices.WorkContext.CurrentCulture
                    );
            }
            return string.Empty;
        }

        public string GetPriceString(double? price, string culture) {
            if (price.HasValue) {
                return SelectedCurrency.PriceAsString(price.Value, culture);
            }
            return string.Empty;
        }
        public string GetPriceString(decimal? price, string culture) {
            if (price.HasValue) {
                return SelectedCurrency.PriceAsString(price.Value, culture);
            }
            return string.Empty;
        }
    }
}
