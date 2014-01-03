using Orchard.Environment.Extensions;
using Orchard.UI.Resources;

namespace Nwazet.Commerce.ResourceManifests {
    [OrchardFeature("Nwazet.Commerce")]
    public class CommerceResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            builder.Add().DefineScript("ShoppingCart").SetDependencies("jQuery").SetUrl("shoppingcart.min.js", "shoppingcart.js");
            builder.Add().DefineScript("MinimumOrderQuantity").SetDependencies("jQuery").SetUrl("minimumorderquantity.min.js", "minimumorderquantity.js");
        }

    }
}