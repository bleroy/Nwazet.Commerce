using Nwazet.Commerce.Models;
using Orchard.ContentManagement.Handlers;

namespace Nwazet.Commerce.Tests.Territories.Handlers {
    public class TerritoryHierachyMockHandler : ContentHandler {
        public TerritoryHierachyMockHandler() {
            Filters.Add(new ActivatingFilter<TerritoryHierarchyPart>("HierarchyType0"));
            Filters.Add(new ActivatingFilter<TerritoryHierarchyPart>("HierarchyType1"));
            Filters.Add(new ActivatingFilter<TerritoryHierarchyPart>("HierarchyType2"));
        }
    }
}
