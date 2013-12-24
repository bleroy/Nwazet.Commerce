using System;
using System.Linq;
using NUnit.Framework;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Tests.Helpers;
using Nwazet.Commerce.Tests.Stubs;
using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Nwazet.Commerce.Services;
using Orchard;
using Orchard.Security;
using Orchard.Settings;

namespace Nwazet.Commerce.Tests {
    [TestFixture]
    public class PriceServiceTieredPricingTests {
        // Products for all those tests:
        // 1 - $10 (10=$9, 50=$8, 100=$5)
        // 2 - $10 (5=$9, 10=$8, 15=$7)
        // 3 - $10 (10=$9, 50=$8, 100=$5) *override=false
        // 4 = $10 (2=90%, 5=80%, 10=70%)
        // 5 = $10 (10=8.9/90%)

        [Test]
        public void TieredPriceIsNotUsedWhenQuantityBelowMinimumThreshold() {
            var cart = PrepareCart();
            cart.Add(1, 9);
            CheckCart(cart, 90);
        }

        [Test]
        public void CorrectPriceTierIsUsedBasedOnQuantity() {
            var cart = PrepareCart();
            cart.Add(2, 9);
            CheckCart(cart, 81);
        }

        [Test]
        public void TieredPriceIsNotUsedIfProductOverrideFlagIsFalse() {
            var cart = PrepareCart();
            cart.Add(3, 50);
            CheckCart(cart, 500);
        }

        [Test]
        public void TieredPriceIsUsedWhenExactTierQuantityIsOrdered() {
            var cart = PrepareCart();
            cart.Add(1, 50);
            CheckCart(cart, 400);
        }

        [Test]
        public void CorrectPercentageBasedPriceTierIsUsedBasedOnQuantity() {
            var cart = PrepareCart();
            cart.Add(4, 4);
            CheckCart(cart, 36);
        }

        [Test]
        public void AbsolutePriceIsUsedIfBothAbsoluteAndPercentageExist() {
            var cart = PrepareCart();
            cart.Add(5, 11);
            CheckCart(cart, 97.9);
        }

        // TODO: Additional tiered pricing tests
        // -------------------------------------
        // Don't use tiered pricing if no site wide default and allow override site setting is false
        // Site wide defaults are use if site wide is enabled and no product override is specified
        // Site wide defaults are not used if site wide is disabled
        // Site wide defaults are not used if product override is specified

        private static readonly IWorkContextAccessor WorkContextAccessor =
            new WorkContextAccessorStub(new Dictionary<Type, object> {
                {typeof(IUser),
                    new UserStub(
                        "Joe",
                        "joe@orchardproject.net",
                        new[] {
                            "Moderator",
                            "Reseller",
                            "Customer"})},
                {typeof(ISite), new SiteStub(true, false, new List<PriceTier>()) }
            });

        private static readonly ProductStub[] Products = new[] {
            new ProductStub(1) {Price = 10, 
                                OverrideTieredPricing = true, 
                                PriceTiers = new List<PriceTier>() {
                                    new PriceTier() { Quantity = 10, Price = 9.0 },
                                    new PriceTier() { Quantity = 50, Price = 8.0 },
                                    new PriceTier() { Quantity = 100, Price = 5.0 }
                                }},
            new ProductStub(2) {Price = 10, 
                                OverrideTieredPricing = true, 
                                PriceTiers = new List<PriceTier>() {
                                    new PriceTier() { Quantity = 5, Price = 9.0 },
                                    new PriceTier() { Quantity = 10, Price = 8.0 },
                                    new PriceTier() { Quantity = 15, Price = 7.0 }
                                }},
            new ProductStub(3) {Price = 10, 
                                OverrideTieredPricing = false, 
                                PriceTiers = new List<PriceTier>() {
                                    new PriceTier() { Quantity = 10, Price = 9.0 },
                                    new PriceTier() { Quantity = 50, Price = 8.0 },
                                    new PriceTier() { Quantity = 100, Price = 5.0 }
                                }},
            new ProductStub(4) {Price = 10, 
                                OverrideTieredPricing = true, 
                                PriceTiers = new List<PriceTier>() {
                                    new PriceTier() { Quantity = 2, PricePercent = 90 },
                                    new PriceTier() { Quantity = 5, PricePercent = 80 },
                                    new PriceTier() { Quantity = 10, PricePercent = 70 }
                                }},
            new ProductStub(5) {Price = 10, 
                                OverrideTieredPricing = true, 
                                PriceTiers = new List<PriceTier>() {
                                    new PriceTier() { Quantity = 10, Price = 8.9, PricePercent = 90 }
                                }}
        };

        private static ShoppingCart PrepareCart() {
            var contentManager = new ContentManagerStub(Products.Cast<IContent>());
            var cartStorage = new FakeCartStorage();
            var priceService = new PriceService(new IPriceProvider[0], WorkContextAccessor, new TieredPriceProvider(WorkContextAccessor));
            var cart = new ShoppingCart(contentManager, cartStorage, priceService, null, null);

            return cart;
        }

        private static void CheckCart(IShoppingCart cart, double expectedSubTotal) {
            const double epsilon = 0.001;
            Assert.That(Math.Abs(cart.Subtotal() - expectedSubTotal), Is.LessThan(epsilon));
        }
    }
}
