using System;

namespace Nwazet.Commerce.Models {
    [Serializable]
    public sealed class CheckoutItem {

        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string Title { get; set; }

        public override string ToString() {
            return Quantity + " x " + Title + " " + Price.ToString("C");
        }
    }
}