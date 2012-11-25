using System.Linq;
using NUnit.Framework;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.Tests.Stubs;

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
            var validMethods = ShippingMethodFilter.Filter(shippingMethods, cart).ToList();
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
            var validMethods = ShippingMethodFilter.Filter(shippingMethods, cart).ToList();
            Assert.AreEqual(3, validMethods.Count());
            Assert.AreEqual(14, validMethods.Sum(m => m.Price));
        }

        [Test]
        public void DefaultWeightAndSizeMethodsBothGetSelectedIfRelevant() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Weight = 2, Size = "L"}), 
                new ShoppingCartQuantityProduct(2, new ProductStub {Weight = 1})
            };
            var weightShippingMethod = Helpers.BuildWeightBasedShippingMethod(price: 3);
            var sizeShippingMethod = Helpers.BuildSizeBasedShippingMethod(price: 3);
            var shippingMethods = new IShippingMethod[] { weightShippingMethod, sizeShippingMethod };
            Assert.AreEqual(3, weightShippingMethod.ComputePrice(cart, shippingMethods));
            Assert.AreEqual(3, sizeShippingMethod.ComputePrice(cart, shippingMethods));
        }

        [Test]
        public void NoShippingMethodPriceAppliedIfAllProductsHaveFixedShipping() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Weight = 2, Size = "L", ShippingCost = 1}), 
                new ShoppingCartQuantityProduct(2, new ProductStub {Weight = 1, ShippingCost = 3})
            };
            var weightShippingMethod = Helpers.BuildWeightBasedShippingMethod(price: 3);
            var sizeShippingMethod = Helpers.BuildSizeBasedShippingMethod(price: 3);
            var shippingMethods = new IShippingMethod[] { weightShippingMethod, sizeShippingMethod };
            Assert.AreEqual(7, weightShippingMethod.ComputePrice(cart, shippingMethods));
            Assert.AreEqual(7, sizeShippingMethod.ComputePrice(cart, shippingMethods));
        }
    }
}
