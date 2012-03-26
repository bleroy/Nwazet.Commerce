using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Bundles")]
    public class BundlePart : ContentPart<BundlePartRecord> {
        public IEnumerable<int> ProductIds {
            get { return Record.Products.Select(p => p.ContentItemRecord.Id); }
        }
    }
}
