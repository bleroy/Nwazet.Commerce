using Orchard.Environment.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Nwazet.Commerce.Settings {
    [OrchardFeature("Territories")]
    public class TerritoryHierarchyPartSettings {
        [Required]
        public string TerritoryType { get; set; }
        public bool MayChangeTerritoryTypeOnItem { get; set; }
    }
}
