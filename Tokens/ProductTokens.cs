using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Tokens;

namespace Nwazet.Commerce.Tokens {
    [OrchardFeature("Nwazet.Commerce")]
    public class ProductTokens : ITokenProvider {

        public ProductTokens() {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void Describe(DescribeContext context) {
            context.For("Content", T("Content Items"), T("Content Items"))
                .Token("Sku", T("Sku"), T("The SKU for the product."))
                .Token("Price", T("Price"), T("The price of the product."))
                .Token("IsDigital", T("Is Digital"), T("True if this is a digital product."))
                .Token("ShippingCost", T("Shipping Cost"), T("The fixed shipping cost of the product."))
                .Token("Weight", T("Weight"), T("The weight of the item."))
                ;
        }

        public void Evaluate(EvaluateContext context) {
            context.For<IContent>("Content")
                .Token("Sku", content => content.As<ProductPart>().Sku)
                .Token("Price", content => content.As<ProductPart>().Price)
                .Token("IsDigital", content => content.As<ProductPart>().IsDigital)
                .Token("ShippingCost", content => content.As<ProductPart>().ShippingCost)
                .Token("Weight", content => content.As<ProductPart>().Weight)
                ;
        }
    }
}