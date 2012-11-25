using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace Nwazet.Commerce.Tests.Stubs {
    public class DiscountStub : DiscountPart {
        public DiscountStub(int id = -1) {
            _record.Value = new DiscountPartRecord();
            ContentItem = new ContentItem {
                VersionRecord = new ContentItemVersionRecord {
                    ContentItemRecord = new ContentItemRecord()
                },
                ContentType = "Discount"
            };
            ContentItem.Record.Id = id;
            ContentItem.Weld(this);
        }
    }
}
