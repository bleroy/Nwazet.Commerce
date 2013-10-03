using System;
using System.Collections.Generic;
using System.Linq;
using Nwazet.Commerce.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Services;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.Promotions")]
    public class DiscountPriceProvider : IPriceProvider {
        private readonly IContentManager _contentManager;
        private readonly IWorkContextAccessor _wca;
        private readonly IClock _clock;

        private IEnumerable<IPromotion> _promotions; 

        public DiscountPriceProvider(
            IContentManager contentManager,
            IWorkContextAccessor wca,
            IClock clock) {

            _contentManager = contentManager;
            _wca = wca;
            _clock = clock;
        }

        public string Name { get { return "Discount"; } }
        public string ContentTypeName { get { return "Discount"; } }

        // This is only used in testing, to avoid having to stub routing logic
        public Func<IContent, string> DisplayUrlResolver { get; set; }

        public IEnumerable<IPromotion> GetPromotions() {
            if (_promotions != null) return _promotions;
            return _promotions = _contentManager
                .Query<DiscountPart, DiscountPartRecord>("Discount")
                .List()
                .Select(dp => new Discount(_wca, _clock) {DiscountPart = dp});
        }

        public IEnumerable<ShoppingCartQuantityProduct> GetModifiedPrices(
            ShoppingCartQuantityProduct quantityProduct,
            IEnumerable<ShoppingCartQuantityProduct> cartProducts) {

            var discounts = GetPromotions().Cast<Discount>();
            var cartProductList = cartProducts == null ? null : cartProducts.ToList();
            foreach (var discount in discounts) {
                discount.DiscountPart.DisplayUrlResolver = DisplayUrlResolver;
                // Does the discount apply?
                if (!discount.Applies(quantityProduct, cartProductList)) continue;
                // Discount applies
                yield return discount.Apply(quantityProduct, cartProductList);
            }
        }
    }
}
