using System.Collections.Generic;

namespace Nwazet.Commerce.ViewModels {
    public class DiscountEditorViewModel {
        public string Name { get; set; }
        public string Discount { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; }
        public string EndDate { get; set; }
        public string EndTime { get; set; }
        public int? StartQuantity { get; set; }
        public int? EndQuantity { get; set; }
        public string[] DiscountRoles { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public string Pattern { get; set; }
        public string Comment { get; set; }
    }
}
