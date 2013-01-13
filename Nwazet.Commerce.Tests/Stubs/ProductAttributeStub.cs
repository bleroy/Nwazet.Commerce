using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace Nwazet.Commerce.Tests.Stubs {
    public class ProductAttributeStub : ProductAttributePart {
        public ProductAttributeStub(int id, params string[] attributeValues) {
            Record = new ProductAttributePartRecord();
            AttributeValues = attributeValues;
            ContentItem = new ContentItem {
                VersionRecord = new ContentItemVersionRecord {
                    ContentItemRecord = new ContentItemRecord()
                },
                ContentType = "ProductAttribute"
            };
            ContentItem.Record.Id = id;
            ContentItem.Weld(this);
        }
    }
}
