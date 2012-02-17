using System.Linq;
using NUnit.Framework;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;

namespace Nwazet.Commerce.Tests {
    [TestFixture]
    public class ShippingMethodSelectionTest {
        [Test]
        public void NoSuitableShippingMethodYieldsEmptySet() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Weight = 1})
            };
            var shippingMethods = new[] {
                Helpers.BuildWeightBasedShippingMethod(price: 3, minimumWeight: 0.4, maximumWeight: 0.6),
                Helpers.BuildWeightBasedShippingMethod(price: 7, minimumWeight: 1.1)
            };
            Assert.IsFalse(ShippingMethodFilter.Filter(shippingMethods, cart).Any());
        }

        [Test]
        public void OneSuitableShippingMethodGetsSelected() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Weight = 2})
            };
            var shippingMethods = new[] {
                Helpers.BuildWeightBasedShippingMethod(price: 3, minimumWeight: 0, maximumWeight: 1),
                Helpers.BuildWeightBasedShippingMethod(price: 7, minimumWeight: 1, maximumWeight: 5),
                Helpers.BuildWeightBasedShippingMethod(price: 11, minimumWeight: 5)
            };
            var validMethods = ShippingMethodFilter.Filter(shippingMethods, cart);
            Assert.AreEqual(1, validMethods.Count());
            Assert.AreEqual(7, validMethods.First().Price);
        }

        [Test]
        public void OverlappingMethodsGetSelected() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Weight = 1})
            };
            var shippingMethods = new[] {
                Helpers.BuildWeightBasedShippingMethod(price: 3, minimumWeight: 0, maximumWeight: 1),
                Helpers.BuildWeightBasedShippingMethod(price: 4, minimumWeight: 0.5, maximumWeight: 1.5),
                Helpers.BuildWeightBasedShippingMethod(price: 7, minimumWeight: 1, maximumWeight: 5),
                Helpers.BuildWeightBasedShippingMethod(price: 11, minimumWeight: 5)
            };
            var validMethods = ShippingMethodFilter.Filter(shippingMethods, cart);
            Assert.AreEqual(3, validMethods.Count());
            Assert.AreEqual(14, validMethods.Sum(m => m.Price));
        }
    }
}
