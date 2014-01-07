using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;
using System.Collections.Generic;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Attributes")]
    public class ProductAttributePartRecord : ContentPartRecord {
        public virtual string AttributeValues { get; set; }
    }
}
