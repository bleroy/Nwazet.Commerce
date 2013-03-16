using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Orchard.Caching;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Stripe")]
    public class StripeService : IStripeService {
        private readonly IWorkContextAccessor _wca;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;
        private readonly dynamic _shapeFactory;

        public StripeService(
            IWorkContextAccessor wca, 
            ICacheManager cacheManager, 
            ISignals signals, 
            IShapeFactory shapeFactory) {

            _wca = wca;
            _cacheManager = cacheManager;
            _signals = signals;
            _shapeFactory = shapeFactory;
        }

        public StripeSettingsPart GetSettings() {
            return _cacheManager.Get(
                "StripeSettings",
                ctx => {
                    ctx.Monitor(_signals.When("Stripe.Changed"));
                    var workContext = _wca.GetContext();
                    return (StripeSettingsPart)workContext
                                                  .CurrentSite
                                                  .ContentItem
                                                  .Get(typeof(StripeSettingsPart));
                });
        }

        public dynamic BuildCheckoutButtonShape(
            IEnumerable<dynamic> productShapes,
            IEnumerable<ShoppingCartQuantityProduct> productQuantities,
            IEnumerable<dynamic> shippingMethodShapes,
            IEnumerable<string> custom) {

            var checkoutSettings = GetSettings();

            return _shapeFactory.Stripe(
                CartItems: productShapes,
                ShippingMethods: shippingMethodShapes,
                Custom: custom,
                PublishableKey: checkoutSettings.PublishableKey);
        }
    }
}
