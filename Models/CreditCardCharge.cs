using Orchard.Localization;

namespace Nwazet.Commerce.Models {
    public class CreditCardCharge : ICharge {

        public CreditCardCharge() {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        public string TransactionId { get; set; }
        public string Last4 { get; set; }
        public int ExpirationMonth { get; set; }
        public int ExpirationYear { get; set; }
        public CheckoutError Error { get; set; }
        public string ChargeText {
            get {
                return T("Card **** **** **** {0} expiring {1}/{2}.", Last4, ExpirationMonth, ExpirationYear).ToString();
            }
        }
        public override string ToString() {
            return ChargeText;
        }
    }
}
