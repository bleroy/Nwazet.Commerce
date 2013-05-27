using System.Linq;
using NUnit.Framework;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.Tests.Stubs;

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
            var prices = shippingMethod.ComputePrice(cart, new IShippingMethod[] {shippingMethod}, null, null, null).ToList();
            Assert.That(prices.Count, Is.EqualTo(1));
            Assert.That(prices.First().Price, Is.EqualTo(0));
        }

        [Test]
        public void DigitalProductsDontIncurShippingCosts() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Weight = 2, ShippingCost = 10, IsDigital = true}), 
                new ShoppingCartQuantityProduct(2, new ProductStub {Weight = 1, ShippingCost = 20, IsDigital = true}) 
            };
            var shippingMethod = Helpers.BuildWeightBasedShippingMethod(price: 3);
            var prices = shippingMethod.ComputePrice(cart, new IShippingMethod[] { shippingMethod }, null, null, null).ToList();
            Assert.That(prices.Count, Is.EqualTo(1));
            Assert.That(prices.First().Price, Is.EqualTo(0));
        }

        [Test]
        public void ProductsUnderMinimumGetIgnored() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Weight = 2}), 
                new ShoppingCartQuantityProduct(2, new ProductStub {Weight = 1})
            };
            var shippingMethod = Helpers.BuildWeightBasedShippingMethod(price: 3, minimumWeight: 5);
            Assert.IsFalse(shippingMethod.ComputePrice(cart, new IShippingMethod[] { shippingMethod }, null, null, null).Any());
        }

        [Test]
        public void ProductsOverMaximumWeightGetIgnored() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Weight = 2}), 
                new ShoppingCartQuantityProduct(2, new ProductStub {Weight = 1})
            };
            var shippingMethod = Helpers.BuildWeightBasedShippingMethod(price: 3, maximumWeight: 3);
            Assert.IsFalse(shippingMethod.ComputePrice(cart, new IShippingMethod[] { shippingMethod }, null, null, null).Any());
        }

        [Test]
        public void ProductsInIntervalIncurMethodPrice() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Weight = 4}),
                new ShoppingCartQuantityProduct(2, new ProductStub {Weight = 2})
            };
            var shippingMethod = Helpers.BuildWeightBasedShippingMethod(price: 3, minimumWeight: 5, maximumWeight: 10);
            var prices = shippingMethod.ComputePrice(cart, new IShippingMethod[] { shippingMethod }, null, null, null).ToList();
            Assert.That(prices.Count, Is.EqualTo(1));
            Assert.That(prices.First().Price, Is.EqualTo(3));
        }

        [Test]
        public void ProductsWithFixedShippingCostIncurJustThat() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Weight = 21, ShippingCost = 3}),
                new ShoppingCartQuantityProduct(2, new ProductStub {Weight = 1, ShippingCost = 4})
            };
            var shippingMethod = Helpers.BuildWeightBasedShippingMethod(price: 3);
            var prices = shippingMethod.ComputePrice(cart, new IShippingMethod[] { shippingMethod }, null, null, null).ToList();
            Assert.That(prices.Count, Is.EqualTo(1));
            Assert.That(prices.First().Price, Is.EqualTo(11));
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
            var prices = shippingMethod.ComputePrice(cart, new IShippingMethod[] { shippingMethod }, null, null, null).ToList();
            Assert.That(prices.Count, Is.EqualTo(1));
            Assert.That(prices.First().Price, Is.EqualTo(8 + 3));
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
            var prices = shippingMethod.ComputePrice(cart, new IShippingMethod[] { shippingMethod }, null, null, null).ToList();
            Assert.That(prices.Count, Is.EqualTo(1));
            Assert.That(prices.First().Price, Is.EqualTo(3));
        }
    }
}
