using System;
using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Promotions")]
    public class DiscountPartRecord : ContentPartRecord {
        public virtual string Name { get; set; }
        public virtual string Discount { get; set; }
        public virtual DateTime? StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual int? StartQuantity { get; set; }
        public virtual int? EndQuantity { get; set; }
        public virtual string Roles { get; set; }
        public virtual string Pattern { get; set; }
        public virtual string Comment { get; set; }
    }
}
