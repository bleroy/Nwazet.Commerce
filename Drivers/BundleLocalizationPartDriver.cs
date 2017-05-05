using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
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
            return null;
        }
    }
}
