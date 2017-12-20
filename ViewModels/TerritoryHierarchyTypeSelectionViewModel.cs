using Nwazet.Commerce.Models;
using Orchard.Environment.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Nwazet.Commerce.ViewModels {
    [OrchardFeature("Territories")]
    public class TerritoryHierarchyTypeSelectionViewModel {

        public TerritoryHierarchyPart Part { get; set; }
        public string TerritoryType { get; set; }
        public string TerritoryTypeDisplayName {
            get {
                return TerritoryTypes.ContainsKey(TerritoryType) ?
                    TerritoryTypes[TerritoryType] :
                    TerritoryType;
            }
        }
        // Dictionary><TypeName, Type Display Name>
        public IDictionary<string, string> TerritoryTypes { get; set; }
        public bool MayChangeTerritoryType { get; set; }

        public TerritoryHierarchyTypeSelectionViewModel() {
            TerritoryTypes = new Dictionary<string, string>();
        }

        public IEnumerable<SelectListItem> ListItems() {
            return TerritoryTypes.Select(kvp =>
                new SelectListItem {
                    Selected = kvp.Key == Part.TerritoryType,
                    Text = kvp.Value,
                    Value = kvp.Key
                }
            );
        }
    }
}
