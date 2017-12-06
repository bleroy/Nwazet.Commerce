using Nwazet.Commerce.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace Nwazet.Commerce.ViewModels {
    public class TerritoryPartViewModel {
        public IList<TerritoryInternalRecord> AvailableTerritoryInternalRecords { get; set; }

        [Required]
        public string SelectedRecordId { get; set; }

        public TerritoryHierarchyPart Hierarchy { get; set; }

        public TerritoryPart Parent { get; set; }

        public TerritoryPart Part { get; set; }

        public TerritoryPartViewModel() {
            AvailableTerritoryInternalRecords = new List<TerritoryInternalRecord>();
        }
        
        public IEnumerable<SelectListItem> ListRecords() {
            return AvailableTerritoryInternalRecords
                .Select(tir =>
                new SelectListItem {
                    Selected = (Part == null || Part.Record.TerritoryInternalRecord == null) ?
                        false :
                        Part.Record.TerritoryInternalRecord.Id == tir.Id,
                    Text = tir.Name,
                    Value = tir.Id.ToString()
                });
        }
    }
}
