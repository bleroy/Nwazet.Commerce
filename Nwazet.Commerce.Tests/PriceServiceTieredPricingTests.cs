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
using Orchard.Services;

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
            var cart = PrepareCart(WorkContextAccessorSiteWideDisabledOverrideEnabled);
            cart.Add(1, 9);
            CheckCart(cart, 90);
        }

        [Test]
        public void CorrectPriceTierIsUsedBasedOnQuantity() {
            var cart = PrepareCart(WorkContextAccessorSiteWideDisabledOverrideEnabled);
            cart.Add(2, 9);
            CheckCart(cart, 81);
        }

        [Test]
        public void TieredPriceIsNotUsedIfProductOverrideFlagIsFalse() {
            var cart = PrepareCart(WorkContextAccessorSiteWideDisabledOverrideEnabled);
            cart.Add(3, 50);
            CheckCart(cart, 500);
        }

        [Test]
        public void TieredPriceIsUsedWhenExactTierQuantityIsOrdered() {
            var cart = PrepareCart(WorkContextAccessorSiteWideDisabledOverrideEnabled);
            cart.Add(1, 50);
            CheckCart(cart, 400);
        }

        [Test]
        public void CorrectPercentageBasedPriceTierIsUsedBasedOnQuantity() {
            var cart = PrepareCart(WorkContextAccessorSiteWideDisabledOverrideEnabled);
            cart.Add(4, 4);
            CheckCart(cart, 36);
        }

        [Test]
        public void AbsolutePriceIsUsedIfBothAbsoluteAndPercentageExist() {
            var cart = PrepareCart(WorkContextAccessorSiteWideDisabledOverrideEnabled);
            cart.Add(5, 11);
            CheckCart(cart, 97.9);
        }

        [Test]
        public void SiteWideDefaultsAreUsedIfEnabledAndNoOverrideExists() {
            var cart = PrepareCart(WorkContextAccessorSiteWideEnabledOverrideEnabled);
            cart.Add(6, 10);
            CheckCart(cart, 75);
        }

        [Test]
        public void NoTiersUsedWhenSiteWideDisabledAndProductOverrideEnabledButNotProvided() {
            var cart = PrepareCart(WorkContextAccessorSiteWideDisabledOverrideEnabled);
            cart.Add(6, 10);
            CheckCart(cart, 100);
        }

        [Test]
        public void SiteWideTiersNotUsedIfProductOverrideEnabledAndProvided() {
            var cart = PrepareCart(WorkContextAccessorSiteWideEnabledOverrideDisabled);
            cart.Add(1, 50);
            CheckCart(cart, 500);
        }

        [Test]
        public void DiscountWithQuantityConstraintAppliedToTier() {
            var cart = PrepareCart(WorkContextAccessorSiteWideDisabledOverrideEnabled,
                new DiscountStub[] { new DiscountStub(1) {
                    StartQuantity = 10,
                    EndQuantity = 20,
                    Discount = 5
                }});
            cart.Add(1, 10);
            CheckCart(cart, 40);
        }

        [Test]
        public void DiscountWithQuantityConstraintNotAppliedToTierWhenOutsideRange() {
            var cart = PrepareCart(WorkContextAccessorSiteWideDisabledOverrideEnabled,
                new DiscountStub[] { new DiscountStub(1) {
                    StartQuantity = 10,
                    EndQuantity = 20,
                    Discount = 5
                }});
            cart.Add(1, 100);
            CheckCart(cart, 500);
        }

        private static readonly IClock Now = new FakeClock(new DateTime(2012, 11, 24, 12, 0, 0, DateTimeKind.Utc));

        private static readonly IWorkContextAccessor WorkContextAccessorSiteWideDisabledOverrideEnabled =
            new WorkContextAccessorStub(new Dictionary<Type, object> {
                {typeof(ISite), new SiteStub(true, false, new List<PriceTier>() { 
                    new PriceTier() { Quantity = 10, PricePercent = 75 },
                    new PriceTier() { Quantity = 100, PricePercent = 50 }
                }) }
            });

        private static readonly IWorkContextAccessor WorkContextAccessorSiteWideEnabledOverrideEnabled =
            new WorkContextAccessorStub(new Dictionary<Type, object> {
                {typeof(ISite), new SiteStub(true, true, new List<PriceTier>() { 
                    new PriceTier() { Quantity = 10, PricePercent = 75 },
                    new PriceTier() { Quantity = 100, PricePercent = 50 }
                })
                }
            });

        private static readonly IWorkContextAccessor WorkContextAccessorSiteWideEnabledOverrideDisabled =
            new WorkContextAccessorStub(new Dictionary<Type, object> {
                {typeof(ISite), new SiteStub(false, true, new List<PriceTier>())
                }
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
                                }},
            new ProductStub(6) {Price = 10, 
                                OverrideTieredPricing = false, 
                                PriceTiers = new List<PriceTier>() }
        };

        private static ShoppingCart PrepareCart(IWorkContextAccessor wca, IEnumerable<DiscountStub> discounts = null, IEnumerable<ITaxProvider> taxProviders = null) {

            var contentItems = discounts == null ? Products : Products.Cast<IContent>().Union(discounts);
            var contentManager = new ContentManagerStub(contentItems);
            var cartStorage = new FakeCartStorage();
            var priceProviders = new IPriceProvider[] {
                new DiscountPriceProvider(contentManager, wca, Now) {
                    DisplayUrlResolver = item => ((ProductStub)item).Path
                }
            };
            var priceService = new PriceService(priceProviders, wca, new TieredPriceProvider(wca));
            var cart = new ShoppingCart(contentManager, cartStorage, priceService, null, taxProviders);

            return cart;
        }

        private static void CheckCart(IShoppingCart cart, double expectedSubTotal) {
            const double epsilon = 0.001;
            Assert.That(Math.Abs(cart.Subtotal() - expectedSubTotal), Is.LessThan(epsilon));
        }
    }
}
