using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;
using System.Collections.Generic;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Territories")]
    public class TerritoryPartRecord : ContentPartRecord {

        public TerritoryPartRecord() {
            Children = new List<TerritoryPartRecord>();
        }

        public virtual TerritoryInternalRecord TerritoryInternalRecord { get; set; }

        public virtual TerritoryPartRecord ParentTerritory { get; set; }

        public virtual TerritoryHierarchyPartRecord Hierarchy { get; set; }

        public virtual IList<TerritoryPartRecord> Children { get; set; }
    }
}
