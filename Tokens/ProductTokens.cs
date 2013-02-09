using System;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;

namespace Nwazet.Commerce.Tokens {
    [OrchardFeature("Nwazet.Commerce")]
    public class ProductTokens : ITokenProvider {

        public ProductTokens() {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void Describe(dynamic context) {
            context.For("Content", T("Content Items"), T("Content Items"))
                .Token("Sku", T("Sku"), T("The SKU for the product."))
                .Token("Price", T("Price"), T("The price of the product."))
                .Token("IsDigital", T("Is Digital"), T("True if this is a digital product."))
                .Token("ShippingCost", T("Shipping Cost"), T("The fixed shipping cost of the product."))
                .Token("Weight", T("Weight"), T("The weight of the item."))
                ;
        }

        public void Evaluate(dynamic context) {
            context.For<IContent>("Content")
                .Token("Sku", (Func<IContent, object>)(content => content.As<ProductPart>().Sku))
                .Token("Price", (Func<IContent, object>)(content => content.As<ProductPart>().Price))
                .Token("IsDigital", (Func<IContent, object>)(content => content.As<ProductPart>().IsDigital))
                .Token("ShippingCost", (Func<IContent, object>)(content => content.As<ProductPart>().ShippingCost))
                .Token("Weight", (Func<IContent, object>)(content => content.As<ProductPart>().Weight))
                ;
        }
    }
}