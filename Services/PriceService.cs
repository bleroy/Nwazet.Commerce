using System;
using System.Collections.Generic;
using System.Linq;
using Nwazet.Commerce.Models;

namespace Nwazet.Commerce.Services {
    public class PriceService : IPriceService {
        private const double Epsilon = 0.001;
        private readonly IEnumerable<IPriceProvider> _priceProviders;

        public PriceService(IEnumerable<IPriceProvider> priceProviders) {
            _priceProviders = priceProviders;
        }

        public ShoppingCartQuantityProduct GetDiscountedPrice(
            ShoppingCartQuantityProduct productQuantity,
            IEnumerable<ShoppingCartQuantityProduct> shoppingCartQuantities = null) {

            var modifiedPrices = _priceProviders
                .SelectMany(pp => pp.GetModifiedPrices(productQuantity, shoppingCartQuantities))
                .ToList();
            if (!modifiedPrices.Any()) return productQuantity;
            var result = new ShoppingCartQuantityProduct(productQuantity.Quantity, productQuantity.Product);
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
