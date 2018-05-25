using Nwazet.Commerce.Models;
using Orchard.Environment.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Nwazet.Commerce.ViewModels {
    [OrchardFeature("Nwazet.AdvancedVAT")]
    public class VatConfigurationSiteSettingsPartViewModel {

        public VatConfigurationSiteSettingsPartViewModel() {
            AvailableTerritoryInternalRecords = new List<TerritoryInternalRecord>();
        }

        public VatConfigurationSiteSettingsPartViewModel(string defaultText)
            : this() {
            DefaultText = defaultText;
        }

        private string DefaultText = "None";

        public VatConfigurationPart DefaultVatConfigurationPart { get; set; }

        public int DefaultTerritoryForVatId { get; set; }

        public IList<TerritoryInternalRecord> AvailableTerritoryInternalRecords { get; set; }

        public IEnumerable<SelectListItem> ListTerritories() {
            var result = new List<SelectListItem>() {
                new SelectListItem {
                    Selected = DefaultTerritoryForVatId == 0,
                    Text = DefaultText,
                    Value = "0"
                }
            };
            result.AddRange(AvailableTerritoryInternalRecords
                .Select(tir => new SelectListItem {
                    Selected = DefaultTerritoryForVatId == tir.Id,
                    Text = tir.Name,
                    Value = tir.Id.ToString()
                }));

            return result;
        }
    }
}
