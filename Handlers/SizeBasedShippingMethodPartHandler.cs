using Nwazet.Commerce.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Handlers {
    [OrchardFeature("Nwazet.Shipping")]
    public class SizeBasedShippingMethodPartHandler : ContentHandler {
        public SizeBasedShippingMethodPartHandler(IRepository<SizeBasedShippingMethodPartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}
