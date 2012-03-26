using System.Collections.Generic;
using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Bundles")]
    public class BundlePartRecord : ContentPartRecord {
        public BundlePartRecord() {
            Products = new List<BundleProductsRecord>();
        }

        public virtual IList<BundleProductsRecord> Products { get; set; }
    }
}
