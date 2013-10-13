using Nwazet.Commerce.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Handlers {
    [OrchardFeature("Nwazet.Orders")]
    public class OrderPartHandler : ContentHandler {
        public OrderPartHandler(IRepository<OrderPartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}
