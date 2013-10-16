using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement.FieldStorage.InfosetStorage;

namespace Nwazet.Commerce.Tests.Stubs {
    public class ProductStub : ProductPart {
        public ProductStub(int id = -1, IEnumerable<int> attributeIds = null) {
            Helpers.PreparePart<ProductPart, ProductPartRecord>(this, "Product", id);
            ContentItem.Weld(new InfosetPart());
            ShippingCost = -1;
            if (attributeIds != null) {
                var attrPartRecord = new ProductAttributesPartRecord();
                var attrPart = new ProductAttributesPart {
                    Record = attrPartRecord
                };
                ContentItem.Weld(attrPart);
                attrPart.AttributeIds = attributeIds;
            }
        }

        public ProductStub(int id, string path, IEnumerable<int> attributeIds = null)
            : this(id, attributeIds) {
            Path = path;
        }

        public string Path { get; private set; }
    }
}
