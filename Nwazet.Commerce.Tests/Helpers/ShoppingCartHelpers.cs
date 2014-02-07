using System;
using System.Collections.Generic;
using System.Linq;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.Tests.Stubs;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Security;
using Orchard.Services;

namespace Nwazet.Commerce.Tests.Helpers {
    public class ShoppingCartHelpers {
        private static readonly double[] Prices = { 10, 1.5, 20 };
        private static readonly int[] Quantities = { 3, 6, 5 };
        private static readonly string[] Paths = {"foo/bar", "bar/baz", "foo/baz/glop"};

        private static readonly ProductStub[] Products = {
            new ProductStub(1, Paths[0]) {Price = Prices[0]},
            new ProductStub(2, Paths[1]) {Price = Prices[1]},
            new ProductStub(3, Paths[2]) {Price = Prices[2]}
        };

        public static readonly ShoppingCartQuantityProduct[] OriginalQuantities = {
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
            cart.AddRange(OriginalQuantities.Reverse()
                .Select(q => new ShoppingCartItem(q.Product.Id, q.Quantity)));
        }

        public static ShoppingCart PrepareCart(IEnumerable<DiscountStub> discounts, IEnumerable<ITaxProvider> taxProviders = null) {

            var contentItems = discounts == null ? Products : Products.Cast<IContent>().Union(discounts);
            var contentManager = new ContentManagerStub(contentItems);
            var cartStorage = new FakeCartStorage();
            var priceProviders = new IPriceProvider[] {
                new DiscountPriceProvider(contentManager, WorkContextAccessor, Now) {
                    DisplayUrlResolver = item => ((ProductStub)item).Path
                }
            };
            var priceService = new PriceService(priceProviders, null);
            var cart = new ShoppingCart(contentManager, cartStorage, priceService, null, taxProviders);
            FillCart(cart);

            return cart;
        }

        public static double CartPriceOf(IProduct product, IEnumerable<ShoppingCartQuantityProduct> quantities) {
            return quantities.First(i => i.Product == product).Price;
        }
    }
}