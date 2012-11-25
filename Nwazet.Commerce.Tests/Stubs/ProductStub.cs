using System.Web.Routing;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace Nwazet.Commerce.Tests.Stubs {
    public class ProductStub : ProductPart {
        public ProductStub(int id = -1) {
            Record = new ProductPartRecord();
            ShippingCost = -1;
            ContentItem = new ContentItem {
                VersionRecord = new ContentItemVersionRecord {
                    ContentItemRecord = new ContentItemRecord()
                },
                ContentType = "Product"
            };
            ContentItem.Record.Id = id;
            ContentItem.Weld(this);
        }

        public ProductStub(int id, string path) : this(id) {
            Path = path;
        }

        public string Path { get; private set; }
    }
}
