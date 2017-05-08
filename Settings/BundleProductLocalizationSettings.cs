using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Settings {
    [OrchardFeature("Nwazet.BundlesLocalizationExtension")]
    public class BundleProductLocalizationSettings {
        //these settings will be attached to a BundlePart
        //if the ContentItem has a LocalizationPart

        public bool TryToLocalizeProducts { get; set; }
        public bool RemoveProductsWithoutLocalization { get; set; }
        public bool AddProductQuantitiesWhenLocalizing { get; set; }
        public bool AssertProductsHaveSameCulture { get; set; }
        public bool HideProductsFromEditor { get; set; }

    }
}
