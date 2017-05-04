using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.DisplayManagement.Descriptors;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Shapes {
    [OrchardFeature("Nwazet.AttributesLocalizationExtension")]
    public class ProductAttributesLocalizationAdminShapeProvider : IShapeTableProvider {
        public void Discover(ShapeTableBuilder builder) {
            builder.Describe("ProductAttributesAdminListItem")
                .OnDisplaying(displaying => {
                    displaying.ShapeMetadata.Alternates.Add("ProductAttributesLocalizationAdminListItem");
                });
        }
    }
}
