using System.Collections.Generic;
using Nwazet.Commerce.Services;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.ViewModels {
    [OrchardFeature("Nwazet.Promotions")]
    public class PromotionIndexViewModel {
        public IList<IPriceProvider> PriceProviders { get; set; }
        public IList<IPromotion> Promotions { get; set; }
        public dynamic Pager { get; set; }
    }
}
