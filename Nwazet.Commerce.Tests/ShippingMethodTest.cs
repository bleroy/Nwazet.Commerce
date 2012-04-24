using NUnit.Framework;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;

namespace Nwazet.Commerce.Tests {
    [TestFixture]
    public class ShippingMethodTest {
        [Test]
        public void DefaultWeightAndSizeMethodsBothGetSelectedIfRelevant() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Weight = 2, Size = "L"}), 
                new ShoppingCartQuantityProduct(2, new ProductStub {Weight = 1})
            };
            var weightShippingMethod = Helpers.BuildWeightBasedShippingMethod(price: 3);
            var sizeShippingMethod = Helpers.BuildSizeBasedShippingMethod(price: 3);
            Assert.AreEqual(3, weightShippingMethod.ComputePrice(cart, new IShippingMethod[] { weightShippingMethod, sizeShippingMethod }));
            Assert.AreEqual(3, sizeShippingMethod.ComputePrice(cart, new IShippingMethod[] { weightShippingMethod, sizeShippingMethod }));
        }

        [Test]
        public void NoShippingMethodPriceAppliedIfAllProductsHaveFixedShipping() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Weight = 2, Size = "L", ShippingCost = 1}), 
                new ShoppingCartQuantityProduct(2, new ProductStub {Weight = 1, ShippingCost = 3})
            };
            var weightShippingMethod = Helpers.BuildWeightBasedShippingMethod(price: 3);
            var sizeShippingMethod = Helpers.BuildSizeBasedShippingMethod(price: 3);
            Assert.AreEqual(7, weightShippingMethod.ComputePrice(cart, new IShippingMethod[] { weightShippingMethod, sizeShippingMethod }));
            Assert.AreEqual(7, sizeShippingMethod.ComputePrice(cart, new IShippingMethod[] { weightShippingMethod, sizeShippingMethod }));
        }
    }
}
