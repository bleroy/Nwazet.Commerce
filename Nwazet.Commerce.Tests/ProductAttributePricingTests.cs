using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Nwazet.Commerce.Drivers;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.Tests.Stubs;
using Orchard.ContentManagement;
using Orchard.UI.Notify;
using Orchard.DisplayManagement.Implementation;

namespace Nwazet.Commerce.Tests {
    [TestFixture]
    public class ProductAttributePricingTests {

        [Test]
        public void AttributeValuesWithNoPriceAdjustmentDoNotChangePrice() {
            var cart = PrepareCart();
            cart.Add(1, 10, new Dictionary<int, ProductAttributeValueExtended> { { 10, GetExtendedValue("Blue") }, { 11, GetExtendedValue("M") } });
            CheckCart(cart, 250); // 10 units x $25/unit
        }

        [Test]
        public void AttributePriceAdjustmentIsAppliedToEachUnitWhenLineAdjustmentSetToFalse() {
            var cart = PrepareCart();
            cart.Add(3, 10, new Dictionary<int, ProductAttributeValueExtended> { { 11, GetExtendedValue("L") } });
            CheckCart(cart, 110); // 10 units x ($10/unit + $1 unit adj) = $210 
        }

        [Test]
        public void AttributePriceAdjustmentIsAppliedToLineTotalWhenLineAdjustmentSetToTrue() {
            var cart = PrepareCart();
            cart.Add(2, 5, new Dictionary<int, ProductAttributeValueExtended> { { 10, GetExtendedValue("Green") } });
            CheckCart(cart, 35); // (5 units x $5/unit) + $10 line adj = $35
        }

        [Test]
        public void MixedPerUnitAndLineAdjustmentAreCalculatedCorrectly() {
            var cart = PrepareCart();
            cart.Add(1, 3, new Dictionary<int, ProductAttributeValueExtended> { { 10, GetExtendedValue("Green") }, { 11, GetExtendedValue("XL") } });
            CheckCart(cart, 91); // (3 units x ($25/unit + $2 unit adj)) + $10 line adj = $91 
        }

        [Test]
        public void NegativeAttributePriceAdjustmentReducesUnitPriceWhenLineAdjustmentSetToFalse() {
            var cart = PrepareCart();
            cart.Add(3, 10, new Dictionary<int, ProductAttributeValueExtended> { { 11, GetExtendedValue("XS") } });
            CheckCart(cart, 90); // 10 units x ($10/unit - $1 unit adj) = $90
        }

        [Test]
        public void NegativeAttributePriceAdjustmentReducesLineTotalWhenLineAdjustmentSetToTrue() {
            var cart = PrepareCart();
            cart.Add(2, 10, new Dictionary<int, ProductAttributeValueExtended> { { 10, GetExtendedValue("Red") } });
            CheckCart(cart, 47); // (10 units x $5/unit) - $3 line adj = $47
        }

        [Test]
        public void MultipleLineAdjustmentsAreCumulative() {
            var cart = PrepareCart();
            cart.Add(4, 1, new Dictionary<int, ProductAttributeValueExtended> { { 10, GetExtendedValue("Green") }, { 12, GetExtendedValue("Yes") } });
            CheckCart(cart, 70); // (1 unit x $10/unit) + $60 line adj = $70
        }

        [Test]
        public void MultiplePerUnitAdjustmentsAreCumulative() {
            var cart = PrepareCart();
            cart.Add(5, 10, new Dictionary<int, ProductAttributeValueExtended> { { 11, GetExtendedValue("XXL") }, { 13, GetExtendedValue("With Sparkles") } });
            CheckCart(cart, 110); // 10 units x ($7/unit + $4 unit adj) = 110
        }

        private static readonly ProductStub[] Products = {
            new ProductStub(1, new[] {10, 11}) {Price = 25},
            new ProductStub(2, new[] {10}) {Price = 5},
            new ProductStub(3, new[] {11}) {Price = 10},
            new ProductStub(4, new[] {10, 12}) { Price = 10 },
            new ProductStub(5, new[] {11, 13}) { Price = 7 }
        };

        private static readonly ProductAttributeStub[] ProductAttributes = {
            new ProductAttributeStub(10, new List<ProductAttributeValue> {
                new ProductAttributeValue { Text = "Green", PriceAdjustment=10, IsLineAdjustment=true },
                new ProductAttributeValue { Text = "Blue", PriceAdjustment=0 },
                new ProductAttributeValue { Text = "Red", PriceAdjustment=-3, IsLineAdjustment=true }
            }),
            new ProductAttributeStub(11, new List<ProductAttributeValue> {
                new ProductAttributeValue { Text = "XS", PriceAdjustment=-1 },
                new ProductAttributeValue { Text = "S", PriceAdjustment=0 },
                new ProductAttributeValue { Text = "M", PriceAdjustment=0 },
                new ProductAttributeValue { Text = "L", PriceAdjustment=1 },
                new ProductAttributeValue { Text = "XL", PriceAdjustment=2 },
                new ProductAttributeValue { Text = "XXL", PriceAdjustment=3 }
            }),
            new ProductAttributeStub(12, new List<ProductAttributeValue> {
                new ProductAttributeValue { Text = "Yes", PriceAdjustment=50, IsLineAdjustment=true },
                new ProductAttributeValue { Text = "No", PriceAdjustment=0, IsLineAdjustment=true }
            }),
            new ProductAttributeStub(13, new List<ProductAttributeValue> {
                new ProductAttributeValue { Text = "Without Sparkles", PriceAdjustment=0 },
                new ProductAttributeValue { Text = "With Sparkles", PriceAdjustment=1 }
            })
        };

        private static ShoppingCart PrepareCart() {
            var contentManager = new ContentManagerStub(Products.Cast<IContent>().Union(ProductAttributes));
            var cartStorage = new FakeCartStorage();
            var attributeService = new ProductAttributeService(contentManager);
            var attributeExtensionProviders = new List<IProductAttributeExtensionProvider> { new TextProductAttributeExtensionProvider(new ShapeFactoryStub()) };
            var priceService = new PriceService(new IPriceProvider[0], attributeService);
            var workContextAccessor = new WorkContextAccessorStub(null);
            var currencyProvider = new UseCurrencyFromSiteCultureProvider(workContextAccessor);
            var attributeDriver = new ProductAttributesPartDriver(attributeService, attributeExtensionProviders, contentManager, currencyProvider);
            var cart = new ShoppingCart(contentManager, cartStorage, priceService, new[] { attributeDriver }, null, new Notifier());

            return cart;
        }

        private static void CheckCart(IShoppingCart cart, decimal expectedSubTotal) {
            const double epsilon = 0.001;
            Assert.That(Math.Abs(cart.Subtotal() - expectedSubTotal), Is.LessThan(epsilon));
        }

        private static ProductAttributeValueExtended GetExtendedValue(string value, string extra = null, string provider = null) {
            return new ProductAttributeValueExtended { Value = value, ExtendedValue = extra, ExtensionProvider = provider };
        }
    }
}
