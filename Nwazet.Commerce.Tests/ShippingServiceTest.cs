using System;
using System.Linq;
using NUnit.Framework;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.Tests.Helpers;
using Nwazet.Commerce.Tests.Stubs;

namespace Nwazet.Commerce.Tests {
    [TestFixture]
    public class ShippingServiceTest {
        [Test]
        public void GetShippingOptionsEnumeratesOptions() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Weight = 3})
            };

            var shippingMethod1 = ShippingHelpers.BuildWeightBasedShippingMethod(price: 3);
            shippingMethod1.Name = "Shipping method 1";
            shippingMethod1.ShippingCompany = "Vandelay Import/Export";
            shippingMethod1.ExcludedShippingAreas = "a,b";
            shippingMethod1.IncludedShippingAreas = "c,d,e";

            var shippingMethod2 = ShippingHelpers.BuildWeightBasedShippingMethod(price: 7);
            shippingMethod2.Name = "Shipping method 2";
            shippingMethod2.ShippingCompany = "Northwind Shipping";
            shippingMethod2.ExcludedShippingAreas = "f,g,h";
            shippingMethod2.IncludedShippingAreas = "i,j";
            
            var shippingMethods = new[] { shippingMethod1, shippingMethod2 };
            
            var validMethods = ShippingService.GetShippingOptions(shippingMethods, cart, null, null, null).ToList();
            
            Assert.AreEqual(2, validMethods.Count());
            var firstOption = validMethods.First();
            Assert.That(firstOption.Description, Is.EqualTo(shippingMethod1.Name));
            Assert.That(firstOption.Price, Is.EqualTo(3));
            Assert.That(firstOption.ShippingCompany, Is.EqualTo(shippingMethod1.ShippingCompany));
            Assert.That(String.Join(",", firstOption.ExcludedShippingAreas), Is.EqualTo(shippingMethod1.ExcludedShippingAreas));
            Assert.That(String.Join(",", firstOption.IncludedShippingAreas), Is.EqualTo(shippingMethod1.IncludedShippingAreas));
            var secondOption = validMethods.Skip(1).First();
            Assert.That(secondOption.Description, Is.EqualTo(shippingMethod2.Name));
            Assert.That(secondOption.Price, Is.EqualTo(7));
            Assert.That(secondOption.ShippingCompany, Is.EqualTo(shippingMethod2.ShippingCompany));
            Assert.That(String.Join(",", secondOption.ExcludedShippingAreas), Is.EqualTo(shippingMethod2.ExcludedShippingAreas));
            Assert.That(String.Join(",", secondOption.IncludedShippingAreas), Is.EqualTo(shippingMethod2.IncludedShippingAreas));
        }

        [Test]
        public void DuplicateMethodDoesNotYieldDuplicateOptions()
        {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Weight = 3})
            };

            var shippingMethod1 = ShippingHelpers.BuildWeightBasedShippingMethod(price: 3);
            var shippingMethod2 = ShippingHelpers.BuildWeightBasedShippingMethod(price: 3);

            var shippingMethods = new[] { shippingMethod1, shippingMethod2 };

            var validMethods = ShippingService.GetShippingOptions(shippingMethods, cart, null, null, null).ToList();

            Assert.AreEqual(1, validMethods.Count());
        }

        [Test]
        public void ShippingOptionCanBeRoundTripped() {
            var cart = new[] {
                new ShoppingCartQuantityProduct(1, new ProductStub {Weight = 3})
            };

            var shippingMethod1 = ShippingHelpers.BuildWeightBasedShippingMethod(price: 3);
            shippingMethod1.Name = "Shipping method 1";
            shippingMethod1.ShippingCompany = "Vandelay Import/Export";
            shippingMethod1.ExcludedShippingAreas = "a,b";
            shippingMethod1.IncludedShippingAreas = "c,d,e";

            var shippingMethod2 = ShippingHelpers.BuildWeightBasedShippingMethod(price: 7);
            shippingMethod2.Name = "Shipping method 2";
            shippingMethod2.ShippingCompany = "Northwind Shipping";
            shippingMethod2.ExcludedShippingAreas = "f,g,h";
            shippingMethod2.IncludedShippingAreas = "i,j";

            var shippingMethods = new[] { shippingMethod1, shippingMethod2 };

            var validMethods = ShippingService.GetShippingOptions(shippingMethods, cart, null, null, null).ToList();

            var firstOption = ShippingService.RebuildShippingOption(validMethods.First().FormValue);
            Assert.That(firstOption.Description, Is.EqualTo(shippingMethod1.Name));
            Assert.That(firstOption.Price, Is.EqualTo(3));
            Assert.That(firstOption.ShippingCompany, Is.EqualTo(shippingMethod1.ShippingCompany));
            Assert.That(String.Join(",", firstOption.ExcludedShippingAreas), Is.EqualTo(shippingMethod1.ExcludedShippingAreas));
            Assert.That(String.Join(",", firstOption.IncludedShippingAreas), Is.EqualTo(shippingMethod1.IncludedShippingAreas));
            var secondOption = ShippingService.RebuildShippingOption(validMethods.Skip(1).First().FormValue);
            Assert.That(secondOption.Description, Is.EqualTo(shippingMethod2.Name));
            Assert.That(secondOption.Price, Is.EqualTo(7));
            Assert.That(secondOption.ShippingCompany, Is.EqualTo(shippingMethod2.ShippingCompany));
            Assert.That(String.Join(",", secondOption.ExcludedShippingAreas), Is.EqualTo(shippingMethod2.ExcludedShippingAreas));
            Assert.That(String.Join(",", secondOption.IncludedShippingAreas), Is.EqualTo(shippingMethod2.IncludedShippingAreas));
        }
    }
}
