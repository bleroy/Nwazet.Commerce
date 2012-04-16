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

        public IEnumerable<ProductQuantity> ProductQuantities {
            get {
                return
                    Record.Products.Select(
                        p => new ProductQuantity {
                            Quantity = p.Quantity,
                            ProductId = p.ContentItemRecord.Id
                        });
            }
        }
    }
}
