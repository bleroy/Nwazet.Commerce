using System;
using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.Commerce")]
    public class ProductDiscountPriceProvider : IPriceProvider {
        private const decimal Epsilon = 0.001M;
        public string Name { get { return "ProductDiscount"; } }
        public string ContentTypeName { get { return null; }}

        public IEnumerable<IPromotion> GetPromotions() {
            return new IPromotion[] {};
        }

        public IEnumerable<ShoppingCartQuantityProduct> GetModifiedPrices(
            ShoppingCartQuantityProduct quantityProduct, 
            IEnumerable<ShoppingCartQuantityProduct> cartProducts) {

            if (quantityProduct.Product.DiscountPrice >= 0 &&
                Math.Abs(quantityProduct.Product.Price - quantityProduct.Product.DiscountPrice) > Epsilon) {

                yield return new ShoppingCartQuantityProduct(
                    quantityProduct.Quantity,
                    quantityProduct.Product,
                    quantityProduct.AttributeIdsToValues) {
                    Price = quantityProduct.Product.DiscountPrice
                };
            }
        }
    }
}