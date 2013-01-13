using Nwazet.Commerce.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Handlers {
    [OrchardFeature("Nwazet.Attributes")]
    public class AttributePartHandler : ContentHandler {
        public AttributePartHandler(IRepository<ProductAttributePartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}
