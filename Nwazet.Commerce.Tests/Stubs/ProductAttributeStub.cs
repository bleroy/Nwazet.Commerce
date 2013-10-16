using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.FieldStorage.InfosetStorage;
using Orchard.ContentManagement.Records;

namespace Nwazet.Commerce.Tests.Stubs {
    public class ProductAttributeStub : ProductAttributePart {
        public ProductAttributeStub(int id, params string[] attributeValues) {
            Record = new ProductAttributePartRecord();
            ContentItem = new ContentItem {
                VersionRecord = new ContentItemVersionRecord {
                    ContentItemRecord = new ContentItemRecord()
                },
                ContentType = "ProductAttribute"
            };
            ContentItem.Record.Id = id;
            ContentItem.Weld(this);
            ContentItem.Weld(new InfosetPart());
            AttributeValues = attributeValues;
        }
    }
}
