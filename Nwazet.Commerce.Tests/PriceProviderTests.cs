using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.Tests.Stubs;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Security;
using Orchard.Services;

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
            var cart = PrepareCart(new DiscountStub[] {});

            CheckDiscount(cart, 1, "");
        }

        [Test]
        public void ThirtyOffWholeCatalogYieldsThirtyOffRebate() {
            var discount = new DiscountStub(4) {
                DiscountPercent = 30,
                Comment = "30% off the whole catalog"
            };
            var cart = PrepareCart(new[] {discount});

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
            var cart = PrepareCart(new[] { mediocreDiscount, bestDiscount, betterDiscount });

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
            var cart = PrepareCart(new[] { oldDiscount, futureDiscount });

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
            var cart = PrepareCart(new[] { currentDiscount });

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
            var cart = PrepareCart(new[] { tooLowDiscount, tooHighDiscount });

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
            var cart = PrepareCart(new[] { wideEnoughDiscount });

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
            var cart = PrepareCart(new[] { selectiveDiscount });

            CheckDiscounts(cart, new[] { 1, 0.9, 0.9 }, new[] { "", selectiveDiscount.Comment, selectiveDiscount.Comment });
        }

        [Test]
        public void PatternAppliesToRightItems() {
            var patternDiscount = new DiscountStub(4) {
                DiscountPercent = 10,
                Pattern = "foo/ba",
                Comment = "Pattern discount"
            };
            var cart = PrepareCart(new[] { patternDiscount });

            CheckDiscounts(cart, new[] { 0.9, 1, 0.9 }, new[] { patternDiscount.Comment, "", patternDiscount.Comment });
        }

        [Test]
        public void RoleNotFoundDiscountDoesntApply() {
            var roleDiscount = new DiscountStub(4) {
                DiscountPercent = 10,
                Roles = new[] {"Administrator", "Employee"},
                Comment = "Role discount"
            };
            var cart = PrepareCart(new[] { roleDiscount });

            CheckDiscount(cart, 1, "");
        }

        [Test]
        public void RoleFoundDiscountApplies() {
            var roleDiscount = new DiscountStub(4) {
                DiscountPercent = 10,
                Roles = new[] { "Employee", "Reseller" },
                Comment = "Role discount"
            };
            var cart = PrepareCart(new[] { roleDiscount });

            CheckDiscount(cart, 0.9, roleDiscount.Comment);
        }

        [Test]
        public void AbsoluteDiscountApplies() {
            var absoluteDiscount = new DiscountStub(4) {
                Discount = 10,
                Comment = "Absolute discount"
            };
            var cart = PrepareCart(new[] { absoluteDiscount });

            CheckDiscounts(cart, new[] {0, 0, 0.5}, new[] {absoluteDiscount.Comment, absoluteDiscount.Comment, absoluteDiscount.Comment});
        }

        private static readonly double[] Prices = new[] { 10, 1.5, 20 };
        private static readonly int[] Quantities = new[] { 3, 6, 5 };
        private static readonly string[] Paths = new[] {"foo/bar", "bar/baz", "foo/baz/glop"};

        private static void CheckDiscount(IShoppingCart cart, double discountRate, string comment) {
            const double epsilon = 0.001;
            var expectedSubTotal = Math.Round(OriginalQuantities.Sum(q => q.Quantity * Math.Round(q.Product.Price * discountRate, 2)), 2);
            Assert.That(Math.Abs(cart.Subtotal() - expectedSubTotal), Is.LessThan(epsilon));
            var cartContents = cart.GetProducts().ToList();
            foreach (var shoppingCartQuantityProduct in cartContents) {
                Assert.That(
                    Math.Abs(
                        CartPriceOf(shoppingCartQuantityProduct.Product, cartContents) -
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
                    Math.Abs(
                        CartPriceOf(shoppingCartQuantityProduct.Product, cartContents) - discountedPrice),
                    Is.LessThan(epsilon));
                Assert.That(shoppingCartQuantityProduct.Comment ?? "", Is.EqualTo(comments[i]));
                expectedSubTotal += shoppingCartQuantityProduct.Quantity * discountedPrice;
                i++;
            }
            Assert.That(Math.Abs(cart.Subtotal() - expectedSubTotal), Is.LessThan(epsilon));
        }

        private static readonly ProductStub[] Products = new[] {
            new ProductStub(1, Paths[0]) {Price = Prices[0]},
            new ProductStub(2, Paths[1]) {Price = Prices[1]},
            new ProductStub(3, Paths[2]) {Price = Prices[2]}
        };

        private static readonly ShoppingCartQuantityProduct[] OriginalQuantities = new[] {
            new ShoppingCartQuantityProduct(Quantities[0], Products[0]), 
            new ShoppingCartQuantityProduct(Quantities[1], Products[1]), 
            new ShoppingCartQuantityProduct(Quantities[2], Products[2])
        };

        private static readonly IWorkContextAccessor WorkContextAccessor =
            new WorkContextAccessorStub(new Dictionary<Type, object> {
                {typeof(IUser),
                    new UserStub(
                    "Joe",
                    "joe@orchardproject.net",
                    new[] {
                        "Moderator",
                        "Reseller",
                        "Customer"})}
            });

        private static readonly IClock Now = new FakeClock(new DateTime(2012, 11, 24, 12, 0, 0, DateTimeKind.Utc));

        private static void FillCart(IShoppingCart cart) {
            cart.AddRange(OriginalQuantities
                .Select(q => new ShoppingCartItem(q.Product.Id, q.Quantity)));
        }

        private static ShoppingCart PrepareCart(IEnumerable<DiscountStub> discounts) {

            var contentManager = new ContentManagerStub(Products.Cast<IContent>().Union(discounts));
            var cartStorage = new FakeCartStorage();
            var priceProviders = new IPriceProvider[] {
                new DiscountPriceProvider(contentManager, WorkContextAccessor, Now) {
                    DisplayUrlResolver = item => ((ProductStub)item).Path
                }
            };
            var priceService = new PriceService(priceProviders);
            var cart = new ShoppingCart(contentManager, cartStorage, priceService);
            FillCart(cart);

            return cart;
        }

        private static double CartPriceOf(IProduct product, IEnumerable<ShoppingCartQuantityProduct> quantities) {
            return quantities.First(i => i.Product == product).Price;
        }
    }
}
