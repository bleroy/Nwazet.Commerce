using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Handlers {
    [OrchardFeature("Nwazet.Attributes")]
    public class AttributePartHandler : ContentHandler {
        public AttributePartHandler(IRepository<ProductAttributePartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }

        protected override void GetItemMetadata(GetContentItemMetadataContext context) {
            var part = context.ContentItem.As<ProductAttributePart>();
            if (part != null) {
                context.Metadata.Identity.Add("AttributeName", part.TechnicalName);
            }
        }
    }
}
