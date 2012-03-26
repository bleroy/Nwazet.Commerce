using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Bundles")]
    public class BundleProductsRecord {
        public virtual int Id { get; set; }
        public virtual BundlePartRecord BundlePartRecord { get; set; }
        public virtual ContentItemRecord ContentItemRecord { get; set; }
    }
}
