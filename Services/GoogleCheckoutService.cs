using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Orchard.Caching;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Google.Checkout")]
    public class GoogleCheckoutService : IGoogleCheckoutService {
        private readonly IWorkContextAccessor _wca;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;
        private readonly dynamic _shapeFactory;

        public GoogleCheckoutService(
            IWorkContextAccessor wca, 
            ICacheManager cacheManager, 
            ISignals signals, 
            IShapeFactory shapeFactory) {

            _wca = wca;
            _cacheManager = cacheManager;
            _signals = signals;
            _shapeFactory = shapeFactory;
        }

        public GoogleCheckoutSettingsPart GetSettings() {
            return _cacheManager.Get(
                "GoogleCheckoutSettings",
                ctx => {
                    ctx.Monitor(_signals.When("GoogleCheckout.Changed"));
                    var workContext = _wca.GetContext();
                    return (GoogleCheckoutSettingsPart)workContext
                                                  .CurrentSite
                                                  .ContentItem
                                                  .Get(typeof(GoogleCheckoutSettingsPart));
                });
        }

        public dynamic BuildCheckoutButtonShape(
            IEnumerable<dynamic> productShapes,
            IEnumerable<ShoppingCartQuantityProduct> productQuantities,
            IEnumerable<dynamic> shippingMethodShapes,
            IEnumerable<string> custom) {

            var checkoutSettings = GetSettings();

            return _shapeFactory.GoogleCheckout(
                CartItems: productShapes,
                ShippingMethods: shippingMethodShapes,
                Custom: custom,
                Currency: checkoutSettings.Currency,
                WeightUnit: checkoutSettings.WeightUnit,
                MerchantId: checkoutSettings.MerchantId,
                AnalyticsId: checkoutSettings.AnalyticsId,
                UseSandbox: checkoutSettings.UseSandbox);
        }
    }
}
