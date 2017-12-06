using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;
using Orchard.Environment.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Territories")]
    public class TerritoryHierarchyPart : ContentPart<TerritoryHierarchyPartRecord> {

        public static string PartName = "TerritoryHierarchyPart";

        private readonly LazyField<IEnumerable<ContentItem>> _territories = 
            new LazyField<IEnumerable<ContentItem>>();

        public LazyField<IEnumerable<ContentItem>> TerritoriesField {
            get { return _territories; }
        }

        public IEnumerable<ContentItem> Territories {
            get { return _territories.Value; }
            // no setter, because this is "filled" thatnks to a 1-to-n relationship to TerritoryPartRecords 
            //set { _territories.Value = value; }
        }

        public IEnumerable<ContentItem> FirstLevel {
            get {
                return Territories
                  .Where(ci => ci.As<TerritoryPart>().Record.ParentTerritory == null);
            }
        }

        public string TerritoryType {
            get { return Retrieve(r => r.TerritoryType); }
            set { Store(r => r.TerritoryType, value); }
        }
    }
}
