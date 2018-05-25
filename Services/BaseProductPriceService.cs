using Nwazet.Commerce.Models;
using Orchard.Environment.Extensions;
using System.Collections.Generic;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.Commerce")]
    public abstract class BaseProductPriceService : IProductPriceService {

        public virtual decimal GetOriginalPrice(ProductPart part) {
            return part.Price;
        }

        public virtual decimal GetDiscountPrice(ProductPart part) {
           return part.DiscountPrice;
        }

        public virtual decimal GetPrice(ProductPart part) {
            return part.Price;
        }

        public virtual IEnumerable<PriceTier> GetPriceTiers(ProductPart part) {
            return part.PriceTiers;
        }

        public virtual decimal? GetShippingCost(ProductPart part) {
            return part.ShippingCost;
        }

        public virtual decimal GetPrice(ProductPart part, decimal basePrice) {
            return basePrice;
        }

        public virtual decimal GetPrice(ProductPart part, string country, string zipCode) {
            return part.Price;
        }

        public virtual decimal GetPrice(ProductPart part, decimal basePrice, string country, string zipCode) {
            return basePrice;
        }

        public virtual decimal GetDiscountPrice(ProductPart part, string country, string zipCode) {
            return part.DiscountPrice;
        }
    }
}
