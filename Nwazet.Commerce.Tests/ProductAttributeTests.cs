using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Nwazet.Commerce.Drivers;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.Tests.Stubs;
using Orchard.ContentManagement;

namespace Nwazet.Commerce.Tests {
    [TestFixture]
    public class ProductAttributeTests {
        // Cart content for all those tests:
        // 3 x $ 10 (Black, L)
        // 1 x $ 10 (Black, M)
        // 7 x $ 10 (Blue, XS)
        // 6 x $1.5
        // 5 x $ 20 (S)
        // --------
        //     $219

        [Test]
        public void FindCartItemFindsItemWithNoAttributes() {
            var cart = PrepareCart();

            var item = cart.FindCartItem(2);
            Assert.That(item.AttributeIdsToValues, Is.Null);
            Assert.That(item.ProductId, Is.EqualTo(2));
            Assert.That(item.Quantity, Is.EqualTo(6));
        }

        [Test]
        public void FindCartItemDoesntFindItemWithNoAttributesWhenSpecifyingAttributes() {
            var cart = PrepareCart();

            Assert.That(cart.FindCartItem(2, new Dictionary<int, string> {{10, "Green"}}), Is.Null);
        }

        [Test]
        public void FindCartItemDoesntFindItemWithAttributesWhenSpecifyingNoAttributes() {
            var cart = PrepareCart();

            Assert.That(cart.FindCartItem(1), Is.Null);
            Assert.That(cart.FindCartItem(1, new Dictionary<int, string>()), Is.Null);
        }

        [Test]
        public void FindCartItemDoesntFindItemWithAttributesWhenSpecifyingWrongAttributes() {
            var cart = PrepareCart();

            Assert.That(cart.FindCartItem(1, new Dictionary<int, string> { { 10, "Red" }, { 11, "M" } }), Is.Null);
            Assert.That(cart.FindCartItem(1, new Dictionary<int, string> { { 10, "Green" }, { 11, "S" } }), Is.Null);
        }

        [Test]
        public void FindCartItemFindsItemWithAttributes() {
            var cart = PrepareCart();

            var item = cart.FindCartItem(1, new Dictionary<int, string> { { 10, "Green" }, { 11, "L" } });
            Assert.That(item.ToString(), Is.EqualTo("{3 x 1 (Green, L)}"));
            item = cart.FindCartItem(1, new Dictionary<int, string> { { 10, "Green" }, { 11, "M" } });
            Assert.That(item.ToString(), Is.EqualTo("{1 x 1 (Green, M)}"));
            item = cart.FindCartItem(1, new Dictionary<int, string> { { 10, "Blue" }, { 11, "XS" } });
            Assert.That(item.ToString(), Is.EqualTo("{7 x 1 (Blue, XS)}"));
            item = cart.FindCartItem(3, new Dictionary<int, string> { { 11, "S" } });
            Assert.That(item.ToString(), Is.EqualTo("{5 x 3 (S)}"));
        }

        [Test]
        public void CheckCartWorksOnOriginalCart() {
            var cart = PrepareCart();

            CheckCart(cart, OriginalQuantities);
        }

        [Test]
        public void AddProductWithoutAttributesAddsToExistingQuantity() {
            var cart = PrepareCart();

            cart.Add(2, 3);

            CheckCart(cart, new[] {
                new ShoppingCartQuantityProduct(3, Products[0], new Dictionary<int, string> {{10, "Green" }, { 11, "L" }}), 
                new ShoppingCartQuantityProduct(1, Products[0], new Dictionary<int, string> {{10, "Green" }, { 11, "M" }}), 
                new ShoppingCartQuantityProduct(7, Products[0], new Dictionary<int, string> {{10, "Blue" }, { 11, "XS" }}), 
                new ShoppingCartQuantityProduct(9, Products[1]), 
                new ShoppingCartQuantityProduct(5, Products[2], new Dictionary<int, string> {{11, "S"}})
            });
        }

        [Test]
        public void AddProductWithAttributesAddsToExistingQuantity() {
            var cart = PrepareCart();

            cart.Add(1, 1, new Dictionary<int, string> { { 10, "Green" }, { 11, "M" } });

            CheckCart(cart, new[] {
                new ShoppingCartQuantityProduct(3, Products[0], new Dictionary<int, string> {{10, "Green" }, { 11, "L" }}), 
                new ShoppingCartQuantityProduct(2, Products[0], new Dictionary<int, string> {{10, "Green" }, { 11, "M" }}), 
                new ShoppingCartQuantityProduct(7, Products[0], new Dictionary<int, string> {{10, "Blue" }, { 11, "XS" }}), 
                new ShoppingCartQuantityProduct(6, Products[1]), 
                new ShoppingCartQuantityProduct(5, Products[2], new Dictionary<int, string> {{11, "S"}})
            });
        }

        [Test]
        public void AddProductWithDifferentAttributesAddsToExistingQuantityCreatesLine() {
            var cart = PrepareCart();

            cart.Add(1, 2, new Dictionary<int, string> { { 10, "Red" }, { 11, "M" } });

            CheckCart(cart, new[] {
                new ShoppingCartQuantityProduct(3, Products[0], new Dictionary<int, string> {{10, "Green" }, { 11, "L" }}), 
                new ShoppingCartQuantityProduct(1, Products[0], new Dictionary<int, string> {{10, "Green" }, { 11, "M" }}), 
                new ShoppingCartQuantityProduct(7, Products[0], new Dictionary<int, string> {{10, "Blue" }, { 11, "XS" }}), 
                new ShoppingCartQuantityProduct(2, Products[0], new Dictionary<int, string> {{10, "Red" }, { 11, "M" }}), 
                new ShoppingCartQuantityProduct(6, Products[1]), 
                new ShoppingCartQuantityProduct(5, Products[2], new Dictionary<int, string> {{11, "S"}})
            });
        }

        [Test]
        public void AddProductWithoutAttributesThatsNotAlreadyThereCreatesLine() {
            var cart = PrepareCart();

            cart.Add(4, 8);

            CheckCart(cart, new[] {
                new ShoppingCartQuantityProduct(3, Products[0], new Dictionary<int, string> {{10, "Green" }, { 11, "L" }}), 
                new ShoppingCartQuantityProduct(1, Products[0], new Dictionary<int, string> {{10, "Green" }, { 11, "M" }}), 
                new ShoppingCartQuantityProduct(7, Products[0], new Dictionary<int, string> {{10, "Blue" }, { 11, "XS" }}), 
                new ShoppingCartQuantityProduct(6, Products[1]), 
                new ShoppingCartQuantityProduct(5, Products[2], new Dictionary<int, string> {{11, "S"}}),
                new ShoppingCartQuantityProduct(8, Products[3])
            });
        }

        [Test]
        public void AddProductWithAttributesThatsNotAlreadyThereCreatesLine() {
            var cart = PrepareCart();

            cart.Add(5, 8, new Dictionary<int, string> {{10, "Red"}, {11, "M"}});

            CheckCart(cart, new[] {
                new ShoppingCartQuantityProduct(3, Products[0], new Dictionary<int, string> {{10, "Green" }, { 11, "L" }}), 
                new ShoppingCartQuantityProduct(1, Products[0], new Dictionary<int, string> {{10, "Green" }, { 11, "M" }}), 
                new ShoppingCartQuantityProduct(7, Products[0], new Dictionary<int, string> {{10, "Blue" }, { 11, "XS" }}), 
                new ShoppingCartQuantityProduct(6, Products[1]), 
                new ShoppingCartQuantityProduct(5, Products[2], new Dictionary<int, string> {{11, "S"}}),
                new ShoppingCartQuantityProduct(8, Products[4], new Dictionary<int, string> {{10, "Red"}, {11, "M"}})
            });
        }

        [Test]
        public void RemoveProductWithAttributesRemovesLine() {
            var cart = PrepareCart();

            cart.Remove(1, new Dictionary<int, string> { { 10, "Green" }, { 11, "M" } });

            CheckCart(cart, new[] {
                new ShoppingCartQuantityProduct(3, Products[0], new Dictionary<int, string> {{10, "Green" }, { 11, "L" }}), 
                new ShoppingCartQuantityProduct(7, Products[0], new Dictionary<int, string> {{10, "Blue" }, { 11, "XS" }}), 
                new ShoppingCartQuantityProduct(6, Products[1]), 
                new ShoppingCartQuantityProduct(5, Products[2], new Dictionary<int, string> {{11, "S"}})
            });
        }

        [Test]
        public void RemoveProductWithoutAttributesRemovesLine() {
            var cart = PrepareCart();

            cart.Remove(2);

            CheckCart(cart, new[] {
                new ShoppingCartQuantityProduct(3, Products[0], new Dictionary<int, string> {{10, "Green" }, { 11, "L" }}), 
                new ShoppingCartQuantityProduct(1, Products[0], new Dictionary<int, string> {{10, "Green" }, { 11, "M" }}), 
                new ShoppingCartQuantityProduct(7, Products[0], new Dictionary<int, string> {{10, "Blue" }, { 11, "XS" }}), 
                new ShoppingCartQuantityProduct(5, Products[2], new Dictionary<int, string> {{11, "S"}})
            });
        }

        [Test]
        public void RemoveProductWithNonExistingAttributeValuesDoesNothing() {
            var cart = PrepareCart();

            cart.Remove(1, new Dictionary<int, string> { { 10, "Red" }, { 11, "M" } });

            CheckCart(cart, OriginalQuantities);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void AddProductWithBadAttributeValueThrows() {
            var cart = PrepareCart();

            cart.Add(1, 8, new Dictionary<int, string> {{10, "NotAValidColor"}, {11, "M"}});
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void AddProductWithTooFewAttributeValueThrows() {
            var cart = PrepareCart();

            cart.Add(1, 8, new Dictionary<int, string> { { 11, "M" } });
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void AddProductWithTooManyAttributeValueThrows() {
            var cart = PrepareCart();

            cart.Add(2, 8, new Dictionary<int, string> { { 10, "Green" }, { 11, "M" } });
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void AddProductWithAttributeValuesWhereProductHasNoneThrows() {
            var cart = PrepareCart();

            cart.Add(2, 8, new Dictionary<int, string> { { 11, "M" } });
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void AddProductWithDifferentAttributesThrows() {
            var cart = PrepareCart();

            cart.Add(2, 8, new Dictionary<int, string> { { 10, "Green" } });
        }

        private static readonly ProductStub[] Products = new[] {
            new ProductStub(1, new[] {10, 11}) {Price = 10},
            new ProductStub(2, new int[0]) {Price = 1.5},
            new ProductStub(3, new[] {11}) {Price = 20},
            new ProductStub(4, new int[0]) {Price = 15},
            new ProductStub(5, new[] {10, 11}) {Price = 27} 
        };

        private static readonly ProductAttributeStub[] ProductAttributes = new[] {
            new ProductAttributeStub(10, "Green", "Blue", "Red"),
            new ProductAttributeStub(11, "XS", "S", "M", "L", "XL", "XXL")
        };

        private static readonly ShoppingCartQuantityProduct[] OriginalQuantities = new[] {
            new ShoppingCartQuantityProduct(3, Products[0], new Dictionary<int, string> {{10, "Green"}, {11, "L"}}), 
            new ShoppingCartQuantityProduct(1, Products[0], new Dictionary<int, string> {{10, "Green"}, {11, "M"}}), 
            new ShoppingCartQuantityProduct(7, Products[0], new Dictionary<int, string> {{10, "Blue"}, {11, "XS"}}), 
            new ShoppingCartQuantityProduct(6, Products[1]), 
            new ShoppingCartQuantityProduct(5, Products[2], new Dictionary<int, string> {{11, "S"}})
        };

        private static void FillCart(IShoppingCart cart) {
            cart.AddRange(OriginalQuantities
                .Select(q => new ShoppingCartItem(q.Product.Id, q.Quantity, q.AttributeIdsToValues)));
        }

        private static ShoppingCart PrepareCart() {
            var contentManager = new ContentManagerStub(Products.Cast<IContent>().Union(ProductAttributes));
            var cartStorage = new FakeCartStorage();
            var priceService = new PriceService(new IPriceProvider[0]);
            var attributeDriver = new ProductAttributesPartDriver(contentManager);
            var cart = new ShoppingCart(contentManager, cartStorage, priceService, new[] {attributeDriver});
            FillCart(cart);

            return cart;
        }

        private static void CheckCart(IShoppingCart cart, IEnumerable<ShoppingCartQuantityProduct> expectedQuantities) {
            const double epsilon = 0.001;
            var expectedQuantityList = expectedQuantities.ToList();
            var expectedSubTotal = Math.Round(expectedQuantityList.Sum(q => q.Quantity * Math.Round(q.Product.Price, 2)), 2);
            Assert.That(Math.Abs(cart.Subtotal() - expectedSubTotal), Is.LessThan(epsilon));
            var cartContents = cart.GetProducts().ToList();
            Assert.That(cartContents.Count == expectedQuantityList.Count());
            foreach (var shoppingCartQuantityProduct in cartContents) {
                var product = cart.FindCartItem(
                    shoppingCartQuantityProduct.Product.Id, shoppingCartQuantityProduct.AttributeIdsToValues);
                Assert.That(product.Quantity, Is.EqualTo(shoppingCartQuantityProduct.Quantity));
            }
        }
    }
}
