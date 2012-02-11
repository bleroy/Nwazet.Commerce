using System.Collections.Generic;
using System.Linq;
using Nwazet.Commerce.Models;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Google.Checkout")]
    public class GoogleCheckoutService : IGoogleCheckoutService {
        private readonly IWorkContextAccessor _wca;
        private readonly IContentManager _contentManager;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;
        private readonly dynamic _shapeFactory;
        private readonly IEnumerable<IShippingMethodProvider> _shippingMethodProviders;

        public GoogleCheckoutService(
            IEnumerable<IShippingMethodProvider> shippingMethodProviders,
            IWorkContextAccessor wca, 
            IContentManager contentManager,
            ICacheManager cacheManager, 
            ISignals signals, 
            IShapeFactory shapeFactory) {

            _shippingMethodProviders = shippingMethodProviders;
            _wca = wca;
            _contentManager = contentManager;
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

        public dynamic BuildCheckoutButtonShape(IEnumerable<dynamic> productShapes, IEnumerable<ShoppingCartQuantityProduct> productQuantities) {
            var checkoutSettings = GetSettings();

            var validShippingMethods = _shippingMethodProviders
                .SelectMany(p => p.GetShippingMethods())
                .Select(
                    m => _shapeFactory.ShippingMethod(
                        Price: m.ComputePrice(productQuantities),
                        Currency: checkoutSettings.Currency,
                        DisplayName: _contentManager.GetItemMetadata(m).DisplayText,
                        Name: m.Name,
                        ShippingCompany: m.ShippingCompany,
                        IncludedShippingAreas: m.IncludedShippingAreas == null ? null : m.IncludedShippingAreas.Split(','),
                        ExcludedShippingAreas: m.ExcludedShippingAreas == null ? null : m.ExcludedShippingAreas.Split(',')
                             ))
                .Where(x => x.Price >= 0)
                .ToList();

            return _shapeFactory.GoogleCheckout(
                CartItems: productShapes,
                ShippingMethods: validShippingMethods,
                Currency: checkoutSettings.Currency,
                WeightUnit: checkoutSettings.WeightUnit,
                MerchantId: checkoutSettings.MerchantId,
                AnalyticsId: checkoutSettings.AnalyticsId,
                UseSandbox: checkoutSettings.UseSandbox);
        }
    }
}
