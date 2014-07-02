using System;
using System.Linq;
using NUnit.Framework;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Tests.Helpers;
using Nwazet.Commerce.Tests.Stubs;

namespace Nwazet.Commerce.Tests {
    [TestFixture]
    public class PriceProviderTests {
        // Cart content for all those tests:
        // 3 x $ 10
        // 6 x $1.5
        // 5 x $ 20
        // --------
        //     $139

        [Test]
        public void NoProviderYieldsNoPriceChange() {
            var cart = ShoppingCartHelpers.PrepareCart(new DiscountStub[] {});

            CheckDiscount(cart, 1, "");
        }

        [Test]
        public void ThirtyOffWholeCatalogYieldsThirtyOffRebate() {
            var discount = new DiscountStub(4) {
                DiscountPercent = 30,
                Comment = "30% off the whole catalog"
            };
            var cart = ShoppingCartHelpers.PrepareCart(new[] {discount});

            CheckDiscount(cart, 0.7, discount.Comment);
        }

        [Test]
        public void LowestPriceWins() {
            var mediocreDiscount = new DiscountStub(4) {
                DiscountPercent = 5,
                Comment = "Mediocre discount"
            };
            var betterDiscount = new DiscountStub(5) {
                DiscountPercent = 10,
                Comment = "Better discount"
            };
            var bestDiscount = new DiscountStub(6) {
                DiscountPercent = 20,
                Comment = "Best discount"
            };
            var cart = ShoppingCartHelpers.PrepareCart(new[] { mediocreDiscount, bestDiscount, betterDiscount });

            CheckDiscount(cart, 0.8, bestDiscount.Comment);
        }

        [Test]
        public void OldAndFutureDiscountsDontApply() {
            var oldDiscount = new DiscountStub(4) {
                DiscountPercent = 5,
                StartDate = new DateTime(2012, 11, 1, 12, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2012, 11, 2, 12, 0, 0, DateTimeKind.Utc),
                Comment = "Old discount"
            };
            var futureDiscount = new DiscountStub(5) {
                DiscountPercent = 5,
                StartDate = new DateTime(2012, 12, 24, 12, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2012, 12, 25, 12, 0, 0, DateTimeKind.Utc),
                Comment = "Future discount"
            };
            var cart = ShoppingCartHelpers.PrepareCart(new[] { oldDiscount, futureDiscount });

            CheckDiscount(cart, 1, "");
        }

        [Test]
        public void CurrentlyValidDiscountApplies() {
            var currentDiscount = new DiscountStub(4) {
                DiscountPercent = 5,
                StartDate = new DateTime(2012, 11, 24, 10, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2012, 11, 24, 14, 0, 0, DateTimeKind.Utc),
                Comment = "Currently valid discount"
            };
            var cart = ShoppingCartHelpers.PrepareCart(new[] { currentDiscount });

            CheckDiscount(cart, 0.95, currentDiscount.Comment);
        }

        [Test]
        public void TooLowAndTooHighQuantityDiscountDoesNotApply() {
            var tooLowDiscount = new DiscountStub(4) {
                DiscountPercent = 5,
                StartQuantity = 1,
                EndQuantity = 2,
                Comment = "Too low discount"
            };
            var tooHighDiscount = new DiscountStub(5) {
                DiscountPercent = 5,
                StartQuantity = 7,
                EndQuantity = 10,
                Comment = "Too high discount"
            };
            var cart = ShoppingCartHelpers.PrepareCart(new[] { tooLowDiscount, tooHighDiscount });

            CheckDiscount(cart, 1, "");
        }

        [Test]
        public void WideEnoughQuantityDiscountApplies() {
            var wideEnoughDiscount = new DiscountStub(4) {
                DiscountPercent = 10,
                StartQuantity = 3,
                EndQuantity = 6,
                Comment = "Wide enough discount"
            };
            var cart = ShoppingCartHelpers.PrepareCart(new[] { wideEnoughDiscount });

            CheckDiscount(cart, 0.9, wideEnoughDiscount.Comment);
        }

        [Test]
        public void QuantityDiscountAppliesToRightItems() {
            var selectiveDiscount = new DiscountStub(4) {
                DiscountPercent = 10,
                StartQuantity = 4,
                EndQuantity = 6,
                Comment = "4-5 item discount"
            };
            var cart = ShoppingCartHelpers.PrepareCart(new[] { selectiveDiscount });

            CheckDiscounts(cart, new[] { 1, 0.9, 0.9 }, new[] { "", selectiveDiscount.Comment, selectiveDiscount.Comment });
        }

        [Test]
        public void PatternAppliesToRightItems() {
            var patternDiscount = new DiscountStub(4) {
                DiscountPercent = 10,
                Pattern = "foo/ba",
                Comment = "Pattern discount"
            };
            var cart = ShoppingCartHelpers.PrepareCart(new[] { patternDiscount });

            CheckDiscounts(cart, new[] { 0.9, 1, 0.9 }, new[] { patternDiscount.Comment, "", patternDiscount.Comment });
        }

        [Test]
        public void RoleNotFoundDiscountDoesntApply() {
            var roleDiscount = new DiscountStub(4) {
                DiscountPercent = 10,
                Roles = new[] {"Administrator", "Employee"},
                Comment = "Role discount"
            };
            var cart = ShoppingCartHelpers.PrepareCart(new[] { roleDiscount });

            CheckDiscount(cart, 1, "");
        }

        [Test]
        public void RoleFoundDiscountApplies() {
            var roleDiscount = new DiscountStub(4) {
                DiscountPercent = 10,
                Roles = new[] { "Employee", "Reseller" },
                Comment = "Role discount"
            };
            var cart = ShoppingCartHelpers.PrepareCart(new[] { roleDiscount });

            CheckDiscount(cart, 0.9, roleDiscount.Comment);
        }

        [Test]
        public void AbsoluteDiscountApplies() {
            var absoluteDiscount = new DiscountStub(4) {
                Discount = 10,
                Comment = "Absolute discount"
            };
            var cart = ShoppingCartHelpers.PrepareCart(new[] { absoluteDiscount });

            CheckDiscounts(cart, new[] {0, 0, 0.5}, new[] {absoluteDiscount.Comment, absoluteDiscount.Comment, absoluteDiscount.Comment});
        }

        [Test]
        public void ProductDiscountApplies() {
            var cart = ShoppingCartHelpers.PrepareCart(new DiscountStub[] {}, applyProductDiscounts: true);

            CheckDiscounts(cart, new[] { 1, 1, 0.5 }, new[] { "", "", "" });
        }

        private static void CheckDiscount(IShoppingCart cart, double discountRate, string comment) {
            const double epsilon = 0.001;
            var expectedSubTotal = Math.Round(ShoppingCartHelpers.OriginalQuantities.Sum(q => q.Quantity * Math.Round(q.Product.Price * discountRate, 2)), 2);
            Assert.That(Math.Abs(cart.Subtotal() - expectedSubTotal), Is.LessThan(epsilon));
            var cartContents = cart.GetProducts().ToList();
            foreach (var shoppingCartQuantityProduct in cartContents) {
                Assert.That(
                    Math.Abs(ShoppingCartHelpers.CartPriceOf(shoppingCartQuantityProduct.Product, cartContents) -
                        Math.Round(shoppingCartQuantityProduct.Product.Price * discountRate, 2)), Is.LessThan(epsilon));
                Assert.That(shoppingCartQuantityProduct.Comment ?? "", Is.EqualTo(comment));
            }
        }

        private static void CheckDiscounts(IShoppingCart cart, double[] discountRates, string[] comments) {
            const double epsilon = 0.001;
            var cartContents = cart.GetProducts().ToList();
            var i = 0;
            var expectedSubTotal = 0.0;
            foreach (var shoppingCartQuantityProduct in cartContents) {
                var discountedPrice = Math.Round(shoppingCartQuantityProduct.Product.Price*discountRates[i], 2);
                Assert.That(
                    Math.Abs(ShoppingCartHelpers.CartPriceOf(shoppingCartQuantityProduct.Product, cartContents) - discountedPrice),
                    Is.LessThan(epsilon));
                Assert.That(shoppingCartQuantityProduct.Comment ?? "", Is.EqualTo(comments[i]));
                expectedSubTotal += shoppingCartQuantityProduct.Quantity * discountedPrice;
                i++;
            }
            Assert.That(Math.Abs(cart.Subtotal() - expectedSubTotal), Is.LessThan(epsilon));
        }

        // Cart contents:
        // 3 x $ 10
        // 6 x $1.5
        // 5 x $ 20 <- can be discounted to $ 10
        // --------
        //     $139
    }
}
