namespace Nwazet.Commerce.Models {
    public class CreditCardCharge {
        public string TransactionId { get; set; }
        public string Last4 { get; set; }
        public int ExpirationMonth { get; set; }
        public int ExpirationYear { get; set; }
        public CheckoutError Error { get; set; }

        public override string ToString() {
            return "**** **** **** " + Last4 + " " + ExpirationMonth + "/" + ExpirationYear;
        }
    }
}
