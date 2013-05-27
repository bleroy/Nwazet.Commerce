using System.Collections.Generic;

namespace Nwazet.Commerce.Models {
    public class UspsShippingOptionList : List<ShippingOption> {
        public string Prohibitions { get; set; }
        public string Restrictions { get; set; }
        public string Observations { get; set; }
        public string CustomsForms { get; set; }
        public string ExpressMail { get; set; }
        public string AreasServed { get; set; }
        public string AdditionalRestrictions { get; set; }
    }
}
