using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Handlers {
    [OrchardFeature("Nwazet.InventoryBySKU")]
    public class InventoryBySKUSiteSettingsPartHandler : ContentHandler {
        public InventoryBySKUSiteSettingsPartHandler() {
            Filters.Add(new ActivatingFilter<InventoryBySKUSiteSettingsPart>("Site"));
        }
    }
}
