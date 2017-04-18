using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Controllers;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Nwazet.AdvancedSKUManagement")]
    public class AdvancedSKUsSiteSettingPartDriver : ContentPartDriver<AdvancedSKUsSiteSettingPart> {

        protected override string Prefix
        {
            get
            {
                return "AdvancedSKUsSiteSettings";
            }
        }

        protected override DriverResult Editor(AdvancedSKUsSiteSettingPart part, dynamic shapeHelper) {
            return ContentShape("SiteSettings_AdvancedSKUs",
                    () => shapeHelper.EditorTemplate(
                        TemplateName: "SiteSettings/AdvancedSKUs",
                        Model: new AdancedSKUsSiteSettingsViewModel {
                            RequireUniqueSKU = part.RequireUniqueSKU,
                            GenerateSKUAutomatically = part.GenerateSKUAutomatically,
                            SKUPattern = part.SKUPattern,
                            AllowCustomPattern = part.AllowCustomPattern
                        },
                        Prefix: Prefix
                    )
                ).OnGroup("ECommerceSiteSettings");
        }

        protected override DriverResult Editor(AdvancedSKUsSiteSettingPart part, IUpdateModel updater, dynamic shapeHelper) {
            var model = new AdancedSKUsSiteSettingsViewModel();
            if (updater is ECommerceSettingsAdminController && 
                updater.TryUpdateModel(model, Prefix, null, null)) {
                part.RequireUniqueSKU = model.RequireUniqueSKU;
                part.GenerateSKUAutomatically = model.GenerateSKUAutomatically;
                part.SKUPattern = model.SKUPattern;
                part.AllowCustomPattern = model.AllowCustomPattern;
            }
            return Editor(part, shapeHelper);
        }
    }
}
