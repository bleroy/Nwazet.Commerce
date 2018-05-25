using Nwazet.Commerce.Models;
using Orchard.Environment.Extensions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nwazet.Commerce.ViewModels {
    [OrchardFeature("Nwazet.AdvancedVAT")]
    public class VatConfigurationPartViewModel {
        

        [Required]
        public string TaxProductCategory { get; set; }
        public bool IsDefaultCategory { get; set; }
        public decimal DefaultRate { get; set; }
        public int Priority { get; set; }
        

        public VatConfigurationPart Part { get; set; }

        public List<VatConfigurationHierarchySummaryViewModel> ItemizedSummary { get; set; }
    }
}
