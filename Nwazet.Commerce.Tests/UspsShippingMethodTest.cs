using System.Linq;
using NUnit.Framework;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.Tests.Helpers;
using Nwazet.Commerce.Tests.Stubs;

namespace Nwazet.Commerce.Tests {
    [TestFixture]
    public class UspsShippingMethodTest {
        [Test]
        public void DomesticSelectsDomesticPrices() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub()),
                new ShoppingCartQuantityProduct(2, new ProductStub())
            };
            var domesticShippingMethod = ShippingHelpers.BuildUspsShippingMethod();
            domesticShippingMethod.Markup = 10;
            var internationalShippingMethod = ShippingHelpers.BuildUspsShippingMethod();
            internationalShippingMethod.International = true;
            var wca = ShippingHelpers.GetUspsWorkContextAccessor("foo", false, false, 3);

            var domesticPrices = ShippingService.GetShippingOptions(
                new IShippingMethod[] {domesticShippingMethod, internationalShippingMethod},
                cart, Country.UnitedStates, "90220", wca).ToList();
            Assert.That(domesticPrices.Count, Is.EqualTo(1));
            Assert.That(domesticPrices.First().Price, Is.EqualTo(13));
        }

        [Test]
        public void InternationalSelectsInternationalPrices() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub()),
                new ShoppingCartQuantityProduct(2, new ProductStub())
            };
            var domesticShippingMethod = ShippingHelpers.BuildUspsShippingMethod();
            var internationalShippingMethod = ShippingHelpers.BuildUspsShippingMethod();
            internationalShippingMethod.Markup = 7;
            internationalShippingMethod.International = true;
            var wca = ShippingHelpers.GetUspsWorkContextAccessor("foo", false, false, 3);

            var internationalPrices = ShippingService.GetShippingOptions(
                new IShippingMethod[] {domesticShippingMethod, internationalShippingMethod},
                cart, "France", "78400", wca).ToList();
            Assert.That(internationalPrices.Count, Is.EqualTo(1));
            Assert.That(internationalPrices.First().Price, Is.EqualTo(10));
        }

        [Test]
        public void DefaultShippingMethodWorksIfNoProductWithSize() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub()),
                new ShoppingCartQuantityProduct(2, new ProductStub())
            };
            var defaultShippingMethod = ShippingHelpers.BuildUspsShippingMethod();
            var wca = ShippingHelpers.GetUspsWorkContextAccessor("foo", false, false, 3);
            Assert.AreEqual(3,
                            defaultShippingMethod.ComputePrice(cart, new IShippingMethod[] {defaultShippingMethod},
                                                               Country.UnitedStates, "90220", wca).First().Price);
        }

        [Test]
        public void DefaultShippingMethodWorksIfOnlyMethod() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub()),
                new ShoppingCartQuantityProduct(2, new ProductStub {Size = "L"})
            };
            var defaultShippingMethod = ShippingHelpers.BuildUspsShippingMethod();
            var wca = ShippingHelpers.GetUspsWorkContextAccessor("foo", false, false, 3);
            Assert.AreEqual(3,
                            defaultShippingMethod.ComputePrice(cart, new IShippingMethod[] {defaultShippingMethod},
                                                               Country.UnitedStates, "90220", wca).First().Price);
        }

        [Test]
        public void LargeObjectShippingMethodNotSelectedIfNoLargeObject() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Size = "M"}),
                new ShoppingCartQuantityProduct(2, new ProductStub())
            };
            var defaultShippingMethod = ShippingHelpers.BuildUspsShippingMethod();
            var largeShippingMethod = ShippingHelpers.BuildUspsShippingMethod(size: "L", priority: 1);
            var shippingMethods = new IShippingMethod[] {defaultShippingMethod, largeShippingMethod};
            var wca = ShippingHelpers.GetUspsWorkContextAccessor("foo", false, false, 3);
            Assert.AreEqual(3,
                            defaultShippingMethod.ComputePrice(cart, shippingMethods, Country.UnitedStates, "90220",
                                                               wca).First().Price);
            Assert.IsFalse(
                largeShippingMethod.ComputePrice(cart, shippingMethods, Country.UnitedStates, "90220", wca).Any());
        }

        [Test]
        public void LargeObjectShippingMethodSelectedIfAnyLargeObject() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub()),
                new ShoppingCartQuantityProduct(2, new ProductStub {Size = "L"})
            };
            var defaultShippingMethod = ShippingHelpers.BuildUspsShippingMethod();
            var largeShippingMethod = ShippingHelpers.BuildUspsShippingMethod(size: "L", priority: 1);
            var shippingMethods = new IShippingMethod[] {defaultShippingMethod, largeShippingMethod};
            var wca = ShippingHelpers.GetUspsWorkContextAccessor("foo", false, false, 3);
            Assert.IsFalse(
                defaultShippingMethod.ComputePrice(cart, shippingMethods, Country.UnitedStates, "90220", wca).Any());
            Assert.AreEqual(3,
                            largeShippingMethod.ComputePrice(cart, shippingMethods, Country.UnitedStates, "90220",
                                                             wca).First().Price);
        }

        [Test]
        public void MediumObjectShippingMethodSelectedIfNoLargeObject() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub()),
                new ShoppingCartQuantityProduct(2, new ProductStub {Size = "M"}),
                new ShoppingCartQuantityProduct(1, new ProductStub {Size = "S"})
            };
            var defaultShippingMethod = ShippingHelpers.BuildUspsShippingMethod();
            var mediumShippingMethod = ShippingHelpers.BuildUspsShippingMethod(size: "M", priority: 1);
            var largeShippingMethod = ShippingHelpers.BuildUspsShippingMethod(size: "L", priority: 2);
            var shippingMethods = new IShippingMethod[]
            {defaultShippingMethod, mediumShippingMethod, largeShippingMethod};
            var wca = ShippingHelpers.GetUspsWorkContextAccessor("foo", false, false, 3);
            Assert.IsFalse(
                defaultShippingMethod.ComputePrice(cart, shippingMethods, Country.UnitedStates, "90220", wca).Any());
            Assert.AreEqual(3,
                            mediumShippingMethod.ComputePrice(cart, shippingMethods, Country.UnitedStates, "90220",
                                                              wca).First().Price);
            Assert.IsFalse(
                largeShippingMethod.ComputePrice(cart, shippingMethods, Country.UnitedStates, "90220", wca).Any());
        }

        [Test]
        public void LargeObjectShippingMethodSelectedWhenAnyLargeObjectWithThreeMethods() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Size = "L"}),
                new ShoppingCartQuantityProduct(2, new ProductStub {Size = "M"}),
                new ShoppingCartQuantityProduct(1, new ProductStub {Size = "L"})
            };
            var defaultShippingMethod = ShippingHelpers.BuildUspsShippingMethod();
            var mediumShippingMethod = ShippingHelpers.BuildUspsShippingMethod(size: "M", priority: 1);
            var largeShippingMethod = ShippingHelpers.BuildUspsShippingMethod(size: "L", priority: 2);
            var shippingMethods = new IShippingMethod[]
            {defaultShippingMethod, mediumShippingMethod, largeShippingMethod};
            var wca = ShippingHelpers.GetUspsWorkContextAccessor("foo", false, false, 3);
            Assert.IsFalse(
                defaultShippingMethod.ComputePrice(cart, shippingMethods, Country.UnitedStates, "90220", wca).Any());
            Assert.IsFalse(
                mediumShippingMethod.ComputePrice(cart, shippingMethods, Country.UnitedStates, "90220", wca).Any());
            Assert.AreEqual(3,
                            largeShippingMethod.ComputePrice(cart, shippingMethods, Country.UnitedStates, "90220",
                                                             wca).First().Price);
        }

        [Test]
        public void FixedShippingPriceMakesItIntoPrice() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Size = "L"}),
                new ShoppingCartQuantityProduct(2, new ProductStub {ShippingCost = 2, Size = "M"}),
                new ShoppingCartQuantityProduct(1, new ProductStub {Size = "L"})
            };
            var defaultShippingMethod = ShippingHelpers.BuildUspsShippingMethod();
            var mediumShippingMethod = ShippingHelpers.BuildUspsShippingMethod(size: "M", priority: 1);
            var largeShippingMethod = ShippingHelpers.BuildUspsShippingMethod(size: "L", priority: 2);
            var shippingMethods = new IShippingMethod[]
            {defaultShippingMethod, mediumShippingMethod, largeShippingMethod};
            var wca = ShippingHelpers.GetUspsWorkContextAccessor("foo", false, false, 3);
            Assert.IsFalse(
                defaultShippingMethod.ComputePrice(cart, shippingMethods, Country.UnitedStates, "90220", wca).Any());
            Assert.IsFalse(
                mediumShippingMethod.ComputePrice(cart, shippingMethods, Country.UnitedStates, "90220", wca).Any());
            Assert.AreEqual(7,
                            largeShippingMethod.ComputePrice(cart, shippingMethods, Country.UnitedStates, "90220",
                                                             wca).First().Price);
        }

        [Test]
        public void MarkupMakesItIntoPrice() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub())
            };
            var defaultShippingMethod = ShippingHelpers.BuildUspsShippingMethod();
            defaultShippingMethod.Markup = 4;
            var shippingMethods = new IShippingMethod[] {defaultShippingMethod};
            var wca = ShippingHelpers.GetUspsWorkContextAccessor("foo", false, false, 3);
            Assert.AreEqual(7,
                            defaultShippingMethod.ComputePrice(cart, shippingMethods, Country.UnitedStates, "90220",
                                                               wca).First().Price);
        }

        [Test]
        public void WeightAtMaximumWeightPasses() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Weight = 3.0/16}), // For the moment, weight is in pounds here
                new ShoppingCartQuantityProduct(2, new ProductStub {Weight = 2.0/16})
            };
            var defaultShippingMethod = ShippingHelpers.BuildUspsShippingMethod();
            defaultShippingMethod.WeightPaddingInOunces = 1;
            defaultShippingMethod.MaximumWeightInOunces = 8;
            var shippingMethods = new IShippingMethod[] {defaultShippingMethod};
            var wca = ShippingHelpers.GetUspsWorkContextAccessor("foo", false, false, 3);
            var prices = defaultShippingMethod.ComputePrice(cart, shippingMethods, Country.UnitedStates, "90220", wca);
            Assert.AreEqual(3, prices.First().Price);
        }

        [Test]
        public void WeightBelowMaximumWeightPasses() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Weight = 3.0/16}),
                new ShoppingCartQuantityProduct(2, new ProductStub {Weight = 2.0/16})
            };
            var defaultShippingMethod = ShippingHelpers.BuildUspsShippingMethod();
            defaultShippingMethod.WeightPaddingInOunces = 1;
            defaultShippingMethod.MaximumWeightInOunces = 9;
            var shippingMethods = new IShippingMethod[] {defaultShippingMethod};
            var wca = ShippingHelpers.GetUspsWorkContextAccessor("foo", false, false, 3);
            var prices = defaultShippingMethod.ComputePrice(cart, shippingMethods, Country.UnitedStates, "90220", wca);
            Assert.AreEqual(3, prices.First().Price);
        }

        [Test]
        public void WeightAboveMaximumWeightFails() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Weight = 3.0/16}),
                new ShoppingCartQuantityProduct(2, new ProductStub {Weight = 2.0/16})
            };
            var defaultShippingMethod = ShippingHelpers.BuildUspsShippingMethod();
            defaultShippingMethod.WeightPaddingInOunces = 1;
            defaultShippingMethod.MaximumWeightInOunces = 6.9;
            var shippingMethods = new IShippingMethod[] {defaultShippingMethod};
            var wca = ShippingHelpers.GetUspsWorkContextAccessor("foo", false, false, 3);
            var prices = defaultShippingMethod.ComputePrice(cart, shippingMethods, Country.UnitedStates, "90220", wca);
            Assert.That(prices, Is.Empty);
        }

        [Test]
        public void WithNoMaximumWeightAnythingGoes() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Weight = 30.0/16}),
                new ShoppingCartQuantityProduct(2, new ProductStub {Weight = 20.0/16})
            };
            var defaultShippingMethod = ShippingHelpers.BuildUspsShippingMethod();
            defaultShippingMethod.WeightPaddingInOunces = 10;
            var shippingMethods = new IShippingMethod[] {defaultShippingMethod};
            var wca = ShippingHelpers.GetUspsWorkContextAccessor("foo", false, false, 3);
            Assert.AreEqual(3,
                            defaultShippingMethod.ComputePrice(cart, shippingMethods, Country.UnitedStates, "90220",
                                                               wca).First().Price);
        }

        [Test]
        public void MoreThanOneMethodWithSameSizeWorks() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Size = "L"}),
                new ShoppingCartQuantityProduct(2, new ProductStub {Size = "M"}),
                new ShoppingCartQuantityProduct(1, new ProductStub {Size = "L"})
            };
            var defaultDomesticShippingMethod = ShippingHelpers.BuildUspsShippingMethod();
            var defaultInternationalShippingMethod = ShippingHelpers.BuildUspsShippingMethod();
            var largeDomesticShippingMethod = ShippingHelpers.BuildUspsShippingMethod(size: "L", priority: 1);
            var largeInternationalShippingMethod = ShippingHelpers.BuildUspsShippingMethod(size: "L", priority: 1);
            var methods = new IShippingMethod[] {
                defaultDomesticShippingMethod,
                defaultInternationalShippingMethod,
                largeDomesticShippingMethod,
                largeInternationalShippingMethod
            };
            var wca = ShippingHelpers.GetUspsWorkContextAccessor("foo", false, false, 3);
            Assert.IsFalse(
                defaultDomesticShippingMethod.ComputePrice(cart, methods, Country.UnitedStates, "90220", wca).Any());
            Assert.IsFalse(
                defaultInternationalShippingMethod.ComputePrice(cart, methods, Country.UnitedStates, "90220", wca)
                                                  .Any());
            Assert.AreEqual(3,
                            largeDomesticShippingMethod.ComputePrice(cart, methods, Country.UnitedStates, "90220",
                                                                     wca).First().Price);
            Assert.AreEqual(3,
                            largeInternationalShippingMethod.ComputePrice(cart, methods, Country.UnitedStates,
                                                                          "90220", wca).First().Price);
        }
    }
}
