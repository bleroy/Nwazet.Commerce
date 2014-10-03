using Orchard.ContentManagement.Records;
using Orchard.Data.Conventions;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Orders")]
    public class OrderPartRecord : ContentPartRecord {
        public virtual string Status { get; set; }
        [StringLengthMax]
        public virtual string Contents { get; set; }
        [StringLengthMax]
        public virtual string Customer { get; set; }
        [StringLengthMax]
        public virtual string Activity { get; set; }
        public virtual string TrackingUrl { get; set; }
        public virtual string Password { get; set; }
        public virtual bool IsTestOrder { get; set; }
        public virtual int UserId { get; set; }
    }
}
