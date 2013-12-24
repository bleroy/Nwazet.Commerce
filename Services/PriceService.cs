using System;
using System.Collections.Generic;
using System.Linq;
using Nwazet.Commerce.Models;
using Orchard;
using Orchard.ContentManagement;

namespace Nwazet.Commerce.Services {
    public class PriceService : IPriceService {
        private const double Epsilon = 0.001;
        private readonly IEnumerable<IPriceProvider> _priceProviders;
        private readonly ITieredPriceProvider _tieredPriceProvider;

        public PriceService(IEnumerable<IPriceProvider> priceProviders, IWorkContextAccessor wca, ITieredPriceProvider tieredPriceProvider = null) {
            _priceProviders = priceProviders;
            _tieredPriceProvider = tieredPriceProvider;
        }

        public ShoppingCartQuantityProduct GetDiscountedPrice(
            ShoppingCartQuantityProduct productQuantity,
            IEnumerable<ShoppingCartQuantityProduct> shoppingCartQuantities = null) {

            // If tiered pricing is enabled, get the tiered price before applying discount
            if (_tieredPriceProvider != null) {
                productQuantity = _tieredPriceProvider.GetTieredPrice(productQuantity);
            }

            return GetDiscount(productQuantity, shoppingCartQuantities);
        }

        public IEnumerable<PriceTier> GetDiscountedPriceTiers(ProductPart product) {
            var priceTiers = _tieredPriceProvider != null ?_tieredPriceProvider.GetPriceTiers(product) : null;
            //IEnumerable<PriceTier> discountedPriceTiers = null;
            if (priceTiers != null) {
                priceTiers = priceTiers.Select(t => new PriceTier() { Quantity = t.Quantity, Price = GetDiscountedPrice(new ShoppingCartQuantityProduct(t.Quantity, product)).Price });
            }
            return priceTiers;
        }

        private ShoppingCartQuantityProduct GetDiscount(ShoppingCartQuantityProduct productQuantity,
            IEnumerable<ShoppingCartQuantityProduct> shoppingCartQuantities = null) {
            var modifiedPrices = _priceProviders
                    .SelectMany(pp => pp.GetModifiedPrices(productQuantity, shoppingCartQuantities))
                    .ToList();
            if (!modifiedPrices.Any()) return productQuantity;
            var result = new ShoppingCartQuantityProduct(productQuantity.Quantity, productQuantity.Product, productQuantity.AttributeIdsToValues);
            var minPrice = modifiedPrices.Min(mp => mp.Price);
            result.Price = minPrice;
            var lowestPrice = modifiedPrices.FirstOrDefault(mp => Math.Abs(mp.Price - minPrice) < Epsilon);
            if (lowestPrice != null) {
                result.Comment = lowestPrice.Comment;
            }
            return result;
        }
    }
}
