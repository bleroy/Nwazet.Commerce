using Nwazet.Commerce.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Handlers {
    [OrchardFeature("Usps.Shipping")]
    public class UspsShippingMethodPartHandler : ContentHandler
    {
        public UspsShippingMethodPartHandler(IRepository<UspsShippingMethodPartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}
