using Nwazet.Commerce.Models;
using Orchard.ContentManagement.Handlers;

namespace Nwazet.Commerce.Tests.Territories.Handlers {
    public class TerritoryMockHandler : ContentHandler {
        public TerritoryMockHandler() {
            Filters.Add(new ActivatingFilter<TerritoryPart>("TerritoryType0"));
            Filters.Add(new ActivatingFilter<TerritoryPart>("TerritoryType1"));
            Filters.Add(new ActivatingFilter<TerritoryPart>("TerritoryType2"));
        }
    }
}
