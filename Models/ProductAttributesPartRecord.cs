using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Attributes")]
    public class ProductAttributesPartRecord : ContentPartRecord {
        public virtual string Attributes { get; set; }
    }
}
