using Nwazet.Commerce.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Handlers {
    [OrchardFeature("Nwazet.Commerce")]
    public class DiscountPartHandler : ContentHandler {
        public DiscountPartHandler(IRepository<DiscountPartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}
