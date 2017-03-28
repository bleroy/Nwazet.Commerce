using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.ViewModels {
    [OrchardFeature("Nwazet.AdvancedSKUManagement")]
    public class AdancedSKUsSiteSettingsViewModel {
        public bool RequireUniqueSKU { get; set; }
    }
}
