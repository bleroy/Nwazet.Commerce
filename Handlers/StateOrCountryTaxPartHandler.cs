using Nwazet.Commerce.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Handlers {
    [OrchardFeature("Nwazet.Taxes")]
    public class StateOrCountryPartHandler : ContentHandler {
        public StateOrCountryPartHandler(IRepository<StateOrCountryTaxPartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}