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
        private readonly IContentManager _contentManager;

        public PriceService(IEnumerable<IPriceProvider> priceProviders, IContentManager contentManager, ITieredPriceProvider tieredPriceProvider = null) {
            _priceProviders = priceProviders;
            _tieredPriceProvider = tieredPriceProvider;
            _contentManager = contentManager;
        }

        public ShoppingCartQuantityProduct GetDiscountedPrice(
            ShoppingCartQuantityProduct productQuantity,
            IEnumerable<ShoppingCartQuantityProduct> shoppingCartQuantities = null) {

            // If tiered pricing is enabled, get the tiered price before applying discount
            if (_tieredPriceProvider != null) {
                productQuantity = _tieredPriceProvider.GetTieredPrice(productQuantity);
            }

            var discountedProductQuantity = GetDiscount(productQuantity, shoppingCartQuantities);

            // Adjust price based on attributes selected
            // TODO: Determine if applying attribute price adjustments should be done before or after discounting (thinking after is the right way to go)
            if (discountedProductQuantity.AttributeIdsToValues != null) {
                foreach (var attr in discountedProductQuantity.AttributeIdsToValues) {
                    var value = _contentManager.Get(attr.Key).As<ProductAttributePart>().AttributeValues.Where(v => v.Text.Trim() == attr.Value.Trim()).Single();
                    // If the adjustment is to the line, set it up, otherwise adjust the per item price
                    if (value.IsLineAdjustment) {
                        discountedProductQuantity.LinePriceAdjustment = value.PriceAdjustment;
                    }
                    else {
                        discountedProductQuantity.Price = discountedProductQuantity.Price + value.PriceAdjustment;
                    }                    
                }
            }

            return discountedProductQuantity;
        }

        public IEnumerable<PriceTier> GetDiscountedPriceTiers(ProductPart product) {
            var priceTiers = _tieredPriceProvider != null ?_tieredPriceProvider.GetPriceTiers(product) : null;
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
