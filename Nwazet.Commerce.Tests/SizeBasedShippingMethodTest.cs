using System.Linq;
using NUnit.Framework;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.Tests.Helpers;
using Nwazet.Commerce.Tests.Stubs;

namespace Nwazet.Commerce.Tests {
    [TestFixture]
    public class SizeBasedShippingMethodTest {
        [Test]
        public void DefaultShippingMethodWorksIfNoProductWithSize() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub()), 
                new ShoppingCartQuantityProduct(2, new ProductStub())
            };
            var defaultShippingMethod = ShippingHelpers.BuildSizeBasedShippingMethod(price: 3);
            Assert.AreEqual(3, defaultShippingMethod.ComputePrice(cart, new IShippingMethod[] { defaultShippingMethod }, null, null, null).First().Price);
        }

        [Test]
        public void DefaultShippingMethodWorksIfOnlyMethod() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub()), 
                new ShoppingCartQuantityProduct(2, new ProductStub {Size = "L"})
            };
            var defaultShippingMethod = ShippingHelpers.BuildSizeBasedShippingMethod(price: 3);
            Assert.AreEqual(3, defaultShippingMethod.ComputePrice(cart, new IShippingMethod[] { defaultShippingMethod }, null, null, null).First().Price);
        }

        [Test]
        public void LargeObjectShippingMethodNotSelectedIfNoLargeObject() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Size = "M"}), 
                new ShoppingCartQuantityProduct(2, new ProductStub())
            };
            var defaultShippingMethod = ShippingHelpers.BuildSizeBasedShippingMethod(price: 3);
            var largeShippingMethod = ShippingHelpers.BuildSizeBasedShippingMethod(price: 3, size: "L", priority: 1);
            var shippingMethods = new IShippingMethod[] {defaultShippingMethod, largeShippingMethod};
            Assert.AreEqual(3, defaultShippingMethod.ComputePrice(cart, shippingMethods, null, null, null).First().Price);
            Assert.IsFalse(largeShippingMethod.ComputePrice(cart, shippingMethods, null, null, null).Any());
        }

        [Test]
        public void LargeObjectShippingMethodSelectedIfAnyLargeObject() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub()), 
                new ShoppingCartQuantityProduct(2, new ProductStub {Size = "L"})
            };
            var defaultShippingMethod = ShippingHelpers.BuildSizeBasedShippingMethod(price: 3);
            var largeShippingMethod = ShippingHelpers.BuildSizeBasedShippingMethod(price: 3, size: "L", priority: 1);
            var shippingMethods = new IShippingMethod[] {defaultShippingMethod, largeShippingMethod};
            Assert.IsFalse(defaultShippingMethod.ComputePrice(cart, shippingMethods, null, null, null).Any());
            Assert.AreEqual(3, largeShippingMethod.ComputePrice(cart, shippingMethods, null, null, null).First().Price);
        }

        [Test]
        public void MediumObjectShippingMethodSelectedIfNoLargeObject() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub()), 
                new ShoppingCartQuantityProduct(2, new ProductStub {Size = "M"}),
                new ShoppingCartQuantityProduct(1, new ProductStub {Size = "S"})
            };
            var defaultShippingMethod = ShippingHelpers.BuildSizeBasedShippingMethod(price: 3);
            var mediumShippingMethod = ShippingHelpers.BuildSizeBasedShippingMethod(price: 3, size: "M", priority: 1);
            var largeShippingMethod = ShippingHelpers.BuildSizeBasedShippingMethod(price: 3, size: "L", priority: 2);
            var shippingMethods = new IShippingMethod[] {defaultShippingMethod, mediumShippingMethod, largeShippingMethod};
            Assert.IsFalse(defaultShippingMethod.ComputePrice(cart, shippingMethods, null, null, null).Any());
            Assert.AreEqual(3, mediumShippingMethod.ComputePrice(cart, shippingMethods, null, null, null).First().Price);
            Assert.IsFalse(largeShippingMethod.ComputePrice(cart, shippingMethods, null, null, null).Any());
        }

        [Test]
        public void LargeObjectShippingMethodSelectedWhenAnyLargeObjectWithThreeMethods() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Size = "L"}), 
                new ShoppingCartQuantityProduct(2, new ProductStub {Size = "M"}),
                new ShoppingCartQuantityProduct(1, new ProductStub {Size = "L"})
            };
            var defaultShippingMethod = ShippingHelpers.BuildSizeBasedShippingMethod(price: 3);
            var mediumShippingMethod = ShippingHelpers.BuildSizeBasedShippingMethod(price: 3, size: "M", priority: 1);
            var largeShippingMethod = ShippingHelpers.BuildSizeBasedShippingMethod(price: 3, size: "L", priority: 2);
            var shippingMethods = new IShippingMethod[] {defaultShippingMethod, mediumShippingMethod, largeShippingMethod};
            Assert.IsFalse(defaultShippingMethod.ComputePrice(cart, shippingMethods, null, null, null).Any());
            Assert.IsFalse(mediumShippingMethod.ComputePrice(cart, shippingMethods, null, null, null).Any());
            Assert.AreEqual(3, largeShippingMethod.ComputePrice(cart, shippingMethods, null, null, null).First().Price);
        }

        [Test]
        public void FixedShippingPriceMakesItIntoPrice() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Size = "L"}), 
                new ShoppingCartQuantityProduct(2, new ProductStub {ShippingCost = 2, Size = "M"}),
                new ShoppingCartQuantityProduct(1, new ProductStub {Size = "L"})
            };
            var defaultShippingMethod = ShippingHelpers.BuildSizeBasedShippingMethod(price: 3);
            var mediumShippingMethod = ShippingHelpers.BuildSizeBasedShippingMethod(price: 3, size: "M", priority: 1);
            var largeShippingMethod = ShippingHelpers.BuildSizeBasedShippingMethod(price: 3, size: "L", priority: 2);
            var shippingMethods = new IShippingMethod[] {defaultShippingMethod, mediumShippingMethod, largeShippingMethod};
            Assert.IsFalse(defaultShippingMethod.ComputePrice(cart, shippingMethods, null, null, null).Any());
            Assert.IsFalse(mediumShippingMethod.ComputePrice(cart, shippingMethods, null, null, null).Any());
            Assert.AreEqual(7, largeShippingMethod.ComputePrice(cart, shippingMethods, null, null, null).First().Price);
        }

        [Test]
        public void MoreThanOneMethodWithSameSizeWorks() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Size = "L"}), 
                new ShoppingCartQuantityProduct(2, new ProductStub {Size = "M"}),
                new ShoppingCartQuantityProduct(1, new ProductStub {Size = "L"})
            };
            var defaultDomesticShippingMethod = ShippingHelpers.BuildSizeBasedShippingMethod(price: 3);
            var defaultInternationalShippingMethod = ShippingHelpers.BuildSizeBasedShippingMethod(price: 10);
            var largeDomesticShippingMethod = ShippingHelpers.BuildSizeBasedShippingMethod(price: 5, size: "L", priority: 1);
            var largeInternationalShippingMethod = ShippingHelpers.BuildSizeBasedShippingMethod(price: 15, size: "L", priority: 1);
            var methods = new IShippingMethod[] {
                                                    defaultDomesticShippingMethod,
                                                    defaultInternationalShippingMethod,
                                                    largeDomesticShippingMethod,
                                                    largeInternationalShippingMethod
                                                };
            Assert.IsFalse(defaultDomesticShippingMethod.ComputePrice(cart, methods, null, null, null).Any());
            Assert.IsFalse(defaultInternationalShippingMethod.ComputePrice(cart, methods, null, null, null).Any());
            Assert.AreEqual(5, largeDomesticShippingMethod.ComputePrice(cart, methods, null, null, null).First().Price);
            Assert.AreEqual(15, largeInternationalShippingMethod.ComputePrice(cart, methods, null, null, null).First().Price);
        }
    }
}
