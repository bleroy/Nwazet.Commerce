using Orchard.ContentManagement.Records;

namespace Nwazet.Commerce.Models {
    public class StateOrCountryTaxPartRecord : ContentPartRecord {
        public StateOrCountryTaxPartRecord() {
            Priority = 0;
        }

        public virtual string State { get; set; }
        public virtual string Country { get; set; }
        public virtual double Rate { get; set; }
        public virtual int Priority { get; set; }
    }
}
