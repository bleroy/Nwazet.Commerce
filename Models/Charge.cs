namespace Nwazet.Commerce.Models {
    public class Charge : ICharge {
        public string TransactionId { get; set; }
        public string ChargeText { get; set; }
        public CheckoutError Error { get; set; }
        public override string ToString() {
            return ChargeText;
        }
    }
}
