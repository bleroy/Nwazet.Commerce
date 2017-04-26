using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Nwazet.InventoryBySKU")]
    public class InventoryBySKUSiteSettingsPartDriver : ContentPartDriver<InventoryBySKUSiteSettingsPart>{

        protected override string Prefix
        {
            get
            {
                return "InventoryBySKUSiteSettings";
            }
        }

        protected override DriverResult Editor(InventoryBySKUSiteSettingsPart part, dynamic shapeHelper) {
            return ContentShape("SiteSettings_InventoryBySKU",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "SiteSettings/InventoryBySKU",
                    Model: null,
                    Prefix: Prefix
                    )).OnGroup("ECommerceSiteSettings");
        }

        protected override DriverResult Editor(InventoryBySKUSiteSettingsPart part, IUpdateModel updater, dynamic shapeHelper) {
            return Editor(part, shapeHelper);
        }
    }
}
