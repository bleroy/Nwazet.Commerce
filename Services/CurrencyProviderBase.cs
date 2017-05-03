using Orchard;

namespace Nwazet.Commerce.Services {
    public abstract class CurrencyProviderBase : ICurrencyProvider {
        private readonly IWorkContextAccessor _workContextAccessor;

        public CurrencyProviderBase(
            IWorkContextAccessor workContextAccessor) {

            _workContextAccessor = workContextAccessor;
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
                        _workContextAccessor.GetContext().CurrentCulture
                    );
            }
            return string.Empty;
        }
        public string GetPriceString(decimal? price) {
            if (price.HasValue) {
                return SelectedCurrency.PriceAsString(price.Value,
                        _workContextAccessor.GetContext().CurrentCulture
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
