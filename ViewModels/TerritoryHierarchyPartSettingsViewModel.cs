using Nwazet.Commerce.Settings;
using Orchard.Environment.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Nwazet.Commerce.ViewModels {
    [OrchardFeature("Territories")]
    public class TerritoryHierarchyPartSettingsViewModel {
        public TerritoryHierarchyPartSettings Settings { get; set; }
        // Dictionary><TypeName, Type Display Name>
        public IDictionary<string, string> TerritoryTypes { get; set; }

        public TerritoryHierarchyPartSettingsViewModel() {
            TerritoryTypes = new Dictionary<string, string>();
        }

        public IEnumerable<SelectListItem> ListItems() {
            return TerritoryTypes.Select(kvp =>
                new SelectListItem {
                    Selected = kvp.Key == Settings.TerritoryType,
                    Text = kvp.Value,
                    Value = kvp.Key
                }
            );
        }
    }
}
