using System.Collections.Generic;
using Nwazet.Commerce.Models;

namespace Nwazet.Commerce.ViewModels {
    public class StripeCheckoutViewModel {
        public string PublishableKey { get; set; }
        public IEnumerable<CheckoutItem> CheckoutItems { get; set; }
        public string Token { get; set; }
        public ShippingOption ShippingOption { get; set; }
        public TaxAmount Taxes { get; set; }
        public Address ShippingAddress { get; set; }
        public Address BillingAddress { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string SpecialInstructions { get; set; }
        public string PurchaseOrder { get; set; }
        public double Amount { get; set; }
    }
}
