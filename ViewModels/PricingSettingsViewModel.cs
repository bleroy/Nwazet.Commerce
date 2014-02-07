using Nwazet.Commerce.Models;
using System.Collections.Generic;

namespace Nwazet.Commerce.ViewModels {
    public class PricingSettingsViewModel {
        public bool DefineSiteDefaults { get; set; }
        public bool AllowProductOverrides { get; set; }
        public ICollection<PriceTierViewModel> PriceTiers { get; set; }
    }
}
