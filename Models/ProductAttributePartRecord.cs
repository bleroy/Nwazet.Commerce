using Orchard.ContentManagement.Records;
using Orchard.Data.Conventions;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Attributes")]
    public class ProductAttributePartRecord : ContentPartRecord {
        [StringLengthMax]
        public virtual string AttributeValues { get; set; }
        public virtual int SortOrder { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string TechnicalName { get; set; }
    }
}
