using Nwazet.Commerce.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Handlers {
    [OrchardFeature("Nwazet.Bundles")]
    public class BundlePartHandler : ContentHandler {
        public BundlePartHandler(IRepository<BundlePartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}
