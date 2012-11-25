using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nwazet.Commerce.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Html;
using Orchard.Roles.Models;
using Orchard.Services;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.Promotions")]
    public class DiscountPriceProvider : IPriceProvider {
        private readonly IContentManager _contentManager;
        private readonly IWorkContextAccessor _wca;
        private readonly IClock _clock;

        public DiscountPriceProvider(
            IContentManager contentManager,
            IWorkContextAccessor wca,
            IClock clock) {

            _contentManager = contentManager;
            _wca = wca;
            _clock = clock;
        }

        // This is only used in testing, to avoid having to stub routing logic
        public Func<IContent, string> DisplayUrlResolver { get; set; } 

        public IEnumerable<ShoppingCartQuantityProduct> GetModifiedPrices(
            ShoppingCartQuantityProduct quantityProduct,
            IEnumerable<ShoppingCartQuantityProduct> cartProducts) {

            var discounts = _contentManager.List<DiscountPart>("Discount");
            foreach (var discountPart in discounts) {
                // Does the discount apply?
                var now = _clock.UtcNow;
                if (discountPart.StartDate != null && discountPart.StartDate > now) continue;
                if (discountPart.EndDate != null && discountPart.EndDate < now) continue;
                if (discountPart.StartQuantity != null &&
                    discountPart.StartQuantity > quantityProduct.Quantity)
                    continue;
                if (discountPart.EndQuantity != null &&
                    discountPart.EndQuantity < quantityProduct.Quantity)
                    continue;
                if (!string.IsNullOrWhiteSpace(discountPart.Pattern)) {
                    string path;
                    if (DisplayUrlResolver != null) {
                        path = DisplayUrlResolver(quantityProduct.Product);
                    }
                    else {
                        var urlHelper = new UrlHelper(_wca.GetContext().HttpContext.Request.RequestContext);
                        path = urlHelper.ItemDisplayUrl(quantityProduct.Product);
                    }
                    if (!path.StartsWith(discountPart.Pattern, StringComparison.OrdinalIgnoreCase))
                        continue;
                }
                if (discountPart.Roles.Any()) {
                    var user = _wca.GetContext().CurrentUser;
                    if (user.Has<UserRolesPart>()) {
                        var roles = user.As<UserRolesPart>().Roles;
                        if (!roles.Any(r => discountPart.Roles.Contains(r))) continue;
                    }
                }
                // Discount applies
                var comment = discountPart.Comment; // TODO: tokenize this
                var percent = discountPart.DiscountPercent;
                if (percent != null) {
                    yield return new ShoppingCartQuantityProduct(quantityProduct.Quantity, quantityProduct.Product) {
                        Comment = comment,
                        Price = Math.Round(quantityProduct.Price * (1 - ((double) percent/100)), 2)
                    };
                }
                var discount = discountPart.Discount;
                if (discount != null) {
                    yield return new ShoppingCartQuantityProduct(quantityProduct.Quantity, quantityProduct.Product) {
                        Comment = comment,
                        Price = Math.Round(Math.Max(0, quantityProduct.Price - (double) discount), 2)
                    };
                }
            }
        }
    }
}
