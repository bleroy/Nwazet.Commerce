using System.Collections.Generic;
using Nwazet.Commerce.Models;

namespace Nwazet.Commerce.ViewModels {
    public class StripeCheckoutViewModel {
        public string PublishableKey { get; set; }
        public IEnumerable<ShoppingCartItem> ShoppingCartItems { get; set; }
        public string Token { get; set; }
        public string ShippingHonorific { get; set; }
        public string ShippingFirstName { get; set; }
        public string ShippingLastName { get; set; }
        public string ShippingCompany { get; set; }
        public string ShippingAddress1 { get; set; }
        public string ShippingAddress2 { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingProvince { get; set; }
        public string ShippingPostalCode { get; set; }
        public string ShippingCountry { get; set; }
        public string BillingHonorific { get; set; }
        public string BillingFirstName { get; set; }
        public string BillingLastName { get; set; }
        public string BillingCompany { get; set; }
        public string BillingAddress1 { get; set; }
        public string BillingAddress2 { get; set; }
        public string BillingCity { get; set; }
        public string BillingProvince { get; set; }
        public string BillingPostalCode { get; set; }
        public string BillingCountry { get; set; }
        public string SpecialInstructions { get; set; }
    }
}
