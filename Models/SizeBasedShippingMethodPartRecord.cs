using Orchard.ContentManagement.Records;

namespace Nwazet.Commerce.Models {
    public class SizeBasedShippingMethodPartRecord : ContentPartRecord {
        public SizeBasedShippingMethodPartRecord() {
            Priority = 0;
        }

        public virtual string Name { get; set; }
        public virtual string ShippingCompany { get; set; }
        public virtual double Price { get; set; }
        public virtual string Size { get; set; }
        public virtual int Priority { get; set; }
        public virtual string IncludedShippingAreas { get; set; }
        public virtual string ExcludedShippingAreas { get; set; }
    }
}
