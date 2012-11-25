using NUnit.Framework;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
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
            var defaultShippingMethod = Helpers.BuildSizeBasedShippingMethod(price: 3);
            Assert.AreEqual(3, defaultShippingMethod.ComputePrice(cart, new IShippingMethod[] {defaultShippingMethod}));
        }

        [Test]
        public void DefaultShippingMethodWorksIfOnlyMethod() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub()), 
                new ShoppingCartQuantityProduct(2, new ProductStub {Size = "L"})
            };
            var defaultShippingMethod = Helpers.BuildSizeBasedShippingMethod(price: 3);
            Assert.AreEqual(3, defaultShippingMethod.ComputePrice(cart, new IShippingMethod[] { defaultShippingMethod }));
        }

        [Test]
        public void LargeObjectShippingMethodNotSelectedIfNoLargeObject() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Size = "M"}), 
                new ShoppingCartQuantityProduct(2, new ProductStub())
            };
            var defaultShippingMethod = Helpers.BuildSizeBasedShippingMethod(price: 3);
            var largeShippingMethod = Helpers.BuildSizeBasedShippingMethod(price: 3, size: "L", priority: 1);
            var shippingMethods = new IShippingMethod[] {defaultShippingMethod, largeShippingMethod};
            Assert.AreEqual(3, defaultShippingMethod.ComputePrice(cart, shippingMethods));
            Assert.AreEqual(-1, largeShippingMethod.ComputePrice(cart, shippingMethods));
        }

        [Test]
        public void LargeObjectShippingMethodSelectedIfAnyLargeObject() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub()), 
                new ShoppingCartQuantityProduct(2, new ProductStub {Size = "L"})
            };
            var defaultShippingMethod = Helpers.BuildSizeBasedShippingMethod(price: 3);
            var largeShippingMethod = Helpers.BuildSizeBasedShippingMethod(price: 3, size: "L", priority: 1);
            var shippingMethods = new IShippingMethod[] {defaultShippingMethod, largeShippingMethod};
            Assert.AreEqual(-1, defaultShippingMethod.ComputePrice(cart, shippingMethods));
            Assert.AreEqual(3, largeShippingMethod.ComputePrice(cart, shippingMethods));
        }

        [Test]
        public void MediumObjectShippingMethodSelectedIfNoLargeObject() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub()), 
                new ShoppingCartQuantityProduct(2, new ProductStub {Size = "M"}),
                new ShoppingCartQuantityProduct(1, new ProductStub {Size = "S"})
            };
            var defaultShippingMethod = Helpers.BuildSizeBasedShippingMethod(price: 3);
            var mediumShippingMethod = Helpers.BuildSizeBasedShippingMethod(price: 3, size: "M", priority: 1);
            var largeShippingMethod = Helpers.BuildSizeBasedShippingMethod(price: 3, size: "L", priority: 2);
            var shippingMethods = new IShippingMethod[] {defaultShippingMethod, mediumShippingMethod, largeShippingMethod};
            Assert.AreEqual(-1, defaultShippingMethod.ComputePrice(cart, shippingMethods));
            Assert.AreEqual(3, mediumShippingMethod.ComputePrice(cart, shippingMethods));
            Assert.AreEqual(-1, largeShippingMethod.ComputePrice(cart, shippingMethods));
        }

        [Test]
        public void LargeObjectShippingMethodSelectedWhenAnyLargeObjectWithThreeMethods() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Size = "L"}), 
                new ShoppingCartQuantityProduct(2, new ProductStub {Size = "M"}),
                new ShoppingCartQuantityProduct(1, new ProductStub {Size = "L"})
            };
            var defaultShippingMethod = Helpers.BuildSizeBasedShippingMethod(price: 3);
            var mediumShippingMethod = Helpers.BuildSizeBasedShippingMethod(price: 3, size: "M", priority: 1);
            var largeShippingMethod = Helpers.BuildSizeBasedShippingMethod(price: 3, size: "L", priority: 2);
            var shippingMethods = new IShippingMethod[] {defaultShippingMethod, mediumShippingMethod, largeShippingMethod};
            Assert.AreEqual(-1, defaultShippingMethod.ComputePrice(cart, shippingMethods));
            Assert.AreEqual(-1, mediumShippingMethod.ComputePrice(cart, shippingMethods));
            Assert.AreEqual(3, largeShippingMethod.ComputePrice(cart, shippingMethods));
        }

        [Test]
        public void FixedShippingPriceMakesItIntoPrice() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Size = "L"}), 
                new ShoppingCartQuantityProduct(2, new ProductStub {ShippingCost = 2, Size = "M"}),
                new ShoppingCartQuantityProduct(1, new ProductStub {Size = "L"})
            };
            var defaultShippingMethod = Helpers.BuildSizeBasedShippingMethod(price: 3);
            var mediumShippingMethod = Helpers.BuildSizeBasedShippingMethod(price: 3, size: "M", priority: 1);
            var largeShippingMethod = Helpers.BuildSizeBasedShippingMethod(price: 3, size: "L", priority: 2);
            var shippingMethods = new IShippingMethod[] {defaultShippingMethod, mediumShippingMethod, largeShippingMethod};
            Assert.AreEqual(-1, defaultShippingMethod.ComputePrice(cart, shippingMethods));
            Assert.AreEqual(-1, mediumShippingMethod.ComputePrice(cart, shippingMethods));
            Assert.AreEqual(7, largeShippingMethod.ComputePrice(cart, shippingMethods));
        }

        [Test]
        public void MoreThanOneMethodWithSameSizeWorks() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Size = "L"}), 
                new ShoppingCartQuantityProduct(2, new ProductStub {Size = "M"}),
                new ShoppingCartQuantityProduct(1, new ProductStub {Size = "L"})
            };
            var defaultDomesticShippingMethod = Helpers.BuildSizeBasedShippingMethod(price: 3);
            var defaultInternationalShippingMethod = Helpers.BuildSizeBasedShippingMethod(price: 10);
            var largeDomesticShippingMethod = Helpers.BuildSizeBasedShippingMethod(price: 5, size: "L", priority: 1);
            var largeInternationalShippingMethod = Helpers.BuildSizeBasedShippingMethod(price: 15, size: "L", priority: 1);
            var methods = new IShippingMethod[] {
                                                    defaultDomesticShippingMethod,
                                                    defaultInternationalShippingMethod,
                                                    largeDomesticShippingMethod,
                                                    largeInternationalShippingMethod
                                                };
            Assert.AreEqual(-1, defaultDomesticShippingMethod.ComputePrice(cart, methods));
            Assert.AreEqual(-1, defaultInternationalShippingMethod.ComputePrice(cart, methods));
            Assert.AreEqual(5, largeDomesticShippingMethod.ComputePrice(cart, methods));
            Assert.AreEqual(15, largeInternationalShippingMethod.ComputePrice(cart, methods));
        }
    }
}
