using System.Collections.Generic;
using Nwazet.Commerce.Models;

namespace Nwazet.Commerce.Tests.Stubs {
    public class ProductStub : ProductPart {
        public ProductStub(int id = -1, IEnumerable<int> attributeIds = null) {
            Helpers.PreparePart<ProductPart, ProductPartRecord>(this, "Product", id);
            ShippingCost = -1;
            if (attributeIds != null) {
                var attrPartRecord = new ProductAttributesPartRecord();
                var attrPart = new ProductAttributesPart {
                    Record = attrPartRecord
                };
                attrPart.AttributeIds = attributeIds;
                ContentItem.Weld(attrPart);
            }
        }

        public ProductStub(int id, string path, IEnumerable<int> attributeIds = null)
            : this(id, attributeIds) {
            Path = path;
        }

        public string Path { get; private set; }
    }
}
