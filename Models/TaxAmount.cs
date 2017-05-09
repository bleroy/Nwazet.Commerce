using System;

namespace Nwazet.Commerce.Models {
    [Serializable]
    public class TaxAmount {
        public string Name { get; set; }
        public decimal Amount { get; set; }
    }
}