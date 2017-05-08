using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Settings;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Nwazet.BundlesLocalizationExtension")]
    public class BundleLocalizationPartDriver : ContentPartDriver<BundlePart> {


        protected override string Prefix
        {
            get { return "Bundle"; }
        }

        protected override DriverResult Editor(BundlePart part, dynamic shapeHelper) {
            return ContentShape(
                "Parts_Bundle_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/ProductLocalizationBundle",
                    Prefix: Prefix,
                    Model: part.TypePartDefinition.Settings.GetModel<BundleProductLocalizationSettings>()));
        }

        protected override DriverResult Editor(BundlePart part, IUpdateModel updater, dynamic shapeHelper) {
            return Editor(part, shapeHelper);
        }
    }
}
