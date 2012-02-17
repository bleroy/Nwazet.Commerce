using NUnit.Framework;
using Nwazet.Commerce.Models;

namespace Nwazet.Commerce.Tests {
    [TestFixture]
    public class WeightBasedShippingMethodTest {
        [Test]
        public void FreeShippingWinsOverWeight() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Weight = 2, ShippingCost = 0}), 
                new ShoppingCartQuantityProduct(2, new ProductStub {Weight = 1, ShippingCost = 0})
            };
            var shippingMethod = Helpers.BuildWeightBasedShippingMethod(price: 3);
            Assert.AreEqual(0, shippingMethod.ComputePrice(cart));
        }

        [Test]
        public void DigitalProductsDontIncurShippingCosts() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Weight = 2, ShippingCost = 10, IsDigital = true}), 
                new ShoppingCartQuantityProduct(2, new ProductStub {Weight = 1, ShippingCost = 20, IsDigital = true}) 
            };
            var shippingMethod = Helpers.BuildWeightBasedShippingMethod(price: 3);
            Assert.AreEqual(0, shippingMethod.ComputePrice(cart));
        }

        [Test]
        public void ProductsUnderMinimumGetIgnored() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Weight = 2}), 
                new ShoppingCartQuantityProduct(2, new ProductStub {Weight = 1})
            };
            var shippingMethod = Helpers.BuildWeightBasedShippingMethod(price: 3, minimumWeight: 5);
            Assert.AreEqual(-1, shippingMethod.ComputePrice(cart));
        }

        [Test]
        public void ProductsOverMaximumWeightGetIgnored() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Weight = 2}), 
                new ShoppingCartQuantityProduct(2, new ProductStub {Weight = 1})
            };
            var shippingMethod = Helpers.BuildWeightBasedShippingMethod(price: 3, maximumWeight: 3);
            Assert.AreEqual(-1, shippingMethod.ComputePrice(cart));
        }

        [Test]
        public void ProductsInIntervalIncurMethodPrice() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Weight = 4}),
                new ShoppingCartQuantityProduct(2, new ProductStub {Weight = 2})
            };
            var shippingMethod = Helpers.BuildWeightBasedShippingMethod(price: 3, minimumWeight: 5, maximumWeight: 10);
            Assert.AreEqual(3, shippingMethod.ComputePrice(cart));
        }

        [Test]
        public void ProductsWithFixedShippingCostIncurJustThat() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Weight = 21, ShippingCost = 3}),
                new ShoppingCartQuantityProduct(2, new ProductStub {Weight = 1, ShippingCost = 4})
            };
            var shippingMethod = Helpers.BuildWeightBasedShippingMethod(price: 3);
            Assert.AreEqual(11, shippingMethod.ComputePrice(cart));
        }

        [Test]
        public void ComplexOrderGivesRightShippingCost() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Weight = 1.5}),
                new ShoppingCartQuantityProduct(2, new ProductStub {Weight = 1, ShippingCost = 4}),
                new ShoppingCartQuantityProduct(2, new ProductStub {Weight = 1, IsDigital = true}),
                new ShoppingCartQuantityProduct(3, new ProductStub {Weight = 3})
            };
            var shippingMethod = Helpers.BuildWeightBasedShippingMethod(price: 3, minimumWeight: 10, maximumWeight: 11);
            Assert.AreEqual(8 + 3, shippingMethod.ComputePrice(cart));
        }

        [Test]
        public void FlatRateIsFlat() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Weight = 1.5}),
                new ShoppingCartQuantityProduct(2, new ProductStub {Weight = 1}),
                new ShoppingCartQuantityProduct(2, new ProductStub {Weight = 1, IsDigital = true}),
                new ShoppingCartQuantityProduct(3, new ProductStub {Weight = 3})
            };
            var shippingMethod = Helpers.BuildWeightBasedShippingMethod(price: 3);
            Assert.AreEqual(3, shippingMethod.ComputePrice(cart));
        }
    }
}
