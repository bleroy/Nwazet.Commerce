using System;
using System.Collections.Generic;
using System.Linq;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Tokens;

namespace Nwazet.Commerce.Tokens {
    [OrchardFeature("Nwazet.Orders")]
    public class OrderTokens : ITokenProvider {
        private readonly IOrderService _orderService;
        private readonly IAddressFormatter _addressFormatter;

        public OrderTokens(
            IOrderService orderService,
            IAddressFormatter addressFormatter) {

            _orderService = orderService;
            _addressFormatter = addressFormatter;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void Describe(DescribeContext context) {
            context.For("Order", T("Order"), T("An order for products."))
                .Token("Id", T("ID"), T("Order ID"))
                .Token("EditUrl", T("Edit URL"), T("Edit URL"), "Url")
                .Token("DisplayUrl", T("Display URL"), T("Display URL"), "Url")
                .Token("Items", T("Items"), T("The items ordered."), "CheckoutItems")
                .Token("Status", T("Status"), T("The current status of the order."))
                .Token("ShippingAddress", T("Shipping Address"), T("Shipping Address"), "Address")
                .Token("BillingAddress", T("Billing Address"), T("Billing Address"), "Address")
                .Token("CustomerEmail", T("Customer Email"), T("Customer's email address"), "Email")
                .Token("CustomerPhone", T("Customer Phone"), T("Customer's phone number"), "Tel")
                .Token("SpecialInstructions", T("Special Instructions"), T("Special instructions"), "Text")
                .Token("TrackingUrl", T("Tracking URL"), T("Tracking URL"), "Url")
                .Token("Password", T("Password"), T("Password"), "Text")
                .Token("Shipping", T("Shipping Option"), T("The shipping option chosen by the user."), "ShippingOption")
                .Token("Subtotal", T("Subtotal"), T("The total price of all products in the order, without the tax and shipping costs."))
                .Token("Taxes", T("Taxes"), T("The taxes to be paid on this order."), "TaxAmount")
                .Token("Total", T("Total"), T("The total price of all products in the order, with the tax and shipping costs included."))
                .Token("AmountPaid", T("Amount Paid"), T("The amount that the customer actually paid."))
                .Token("Charge", T("Charge"), T("A charge, on a credit card for example."), "Charge")
                .Token("User", T("User"), T("User associated with the order."), "User")
                .Token("PurchaseOrder", T("Purchase Order"), T("A purchase order, invoice, or order reference number."), "Text");

            context.For("CheckoutItems", T("Checkout Items"), T("The list of product quantities and prices in the order."))
                .Token("Format:*", T("Format:<separator>:<cart format>"), T("Formats the contents of the cart using a format string that uses $quantity for the quantity, $product for the product name (with attributes), and $price for the price. For example {Cart.Items.Format:, :$quantity x $product}"), "Text");

            context.For("Address", T("Address"), T("Address"))
                .Token("Honorific", T("Honorific"), T("Honorific or title, e.g. Mr., Dr., San, etc."), "Text")
                .Token("FirstName", T("First Name"), T("First Name"), "Text")
                .Token("LastName", T("Last Name"), T("Last Name"), "Text")
                .Token("Address1", T("Address 1"), T("First part of the address"), "Text")
                .Token("Address2", T("Address 2"), T("Second part of the address"), "Text")
                .Token("Company", T("Company"), T("Company"), "Text")
                .Token("City", T("City"), T("City"), "Text")
                .Token("Province", T("Province"), T("Province, prefecture, state, or state / republic and region"), "Text")
                .Token("PostalCode", T("PostalCode"), T("Postal or zip code"), "Text")
                .Token("Country", T("Country"), T("Country"), "Text");

            context.For("CheckoutError", T("Checkout Error"), T("An error during the checkout process."))
                .Token("Type", T("Type"), T("Error type"), "Text")
                .Token("Message", T("Message"), T("Error message"), "Text")
                .Token("Code", T("Code"), T("Error code"), "Text");
        }

        public void Evaluate(EvaluateContext context) {
            context.For<OrderPart>("Order")
                .Token("Id", o => o.Id)
                .Token("EditUrl", o => _orderService.GetEditUrl(o))
                .Chain("EditUrl", "Url", o => _orderService.GetEditUrl(o))
                .Token("DisplayUrl", o => _orderService.GetDisplayUrl(o))
                .Chain("DisplayUrl", "Url", o => _orderService.GetDisplayUrl(o))
                .Token("Items", o => string.Join(T(", ").Text, o.Items.Select(i => i.ToString())))
                .Chain("Items", "CheckoutItems", o => o.Items)
                .Token("Status", o => o.Status)
                .Token("ShippingAddress", o => _addressFormatter.Format(o.ShippingAddress))
                .Chain("ShippingAddress", "Address", o => o.ShippingAddress)
                .Token("BillingAddress", o => _addressFormatter.Format(o.BillingAddress))
                .Chain("BillingAddress", "Address", o => o.BillingAddress)
                .Token("CustomerEmail", o => o.CustomerEmail)
                .Chain("CustomerEmail", "Email", o => o.CustomerEmail)
                .Token("CustomerPhone", o => o.CustomerPhone)
                .Chain("CustomerPhone", "Tel", o => o.CustomerPhone)
                .Token("SpecialInstructions", o => o.SpecialInstructions)
                .Chain("SpecialInstructions", "Text", o => o.SpecialInstructions)
                .Token("TrackingUrl", o => o.TrackingUrl)
                .Chain("TrackingUrl", "Url", o => o.TrackingUrl)
                .Token("Password", o => o.Password)
                .Chain("Password", "Text", o => o.Password)
                .Token("Shipping", o => o.ShippingOption.ToString())
                .Chain("Shipping", "ShippingOption", o => o.ShippingOption)
                .Token("Subtotal", o => o.SubTotal.ToString("C"))
                .Token("Taxes", o => o.Taxes.Amount.ToString("C"))
                .Chain("Taxes", "TaxAmount", o => o.Taxes)
                .Token("Total", o => o.Total.ToString("C"))
                .Token("AmountPaid", o => o.AmountPaid.ToString("C"))
                .Token("Charge", o => o.Charge.ToString())
                .Chain("Charge", "Charge", o => o.Charge)
                .Token("User", o => o.User == null ? "" : o.User.UserName)
                .Chain("User", "User", o => o.User)
                .Token("PurchaseOrder", o => o.PurchaseOrder)
                .Chain("PurchaseOrder", "Text", o => o.PurchaseOrder);

            context.For<IEnumerable<CheckoutItem>>("CheckoutItems")
                .Token(s => s.StartsWith("Format:", StringComparison.OrdinalIgnoreCase) ? s.Substring("Format:".Length) : null,
                    (s, q) => {
                        var separator = "";
                        var colonIndex = s.IndexOf(':');
                        if (colonIndex != -1 && s.Length > colonIndex + 1) {
                            separator = s.Substring(0, colonIndex);
                            s = s.Substring(colonIndex + 1);
                        }
                        var format = s
                            .Replace("$quantity", "{0}")
                            .Replace("$product", "{1}")
                            .Replace("$price", "{2}");
                        return String.Join(separator, q.Select(qp =>
                            String.Format(format,
                                qp.Quantity,
                                qp.Title,
                                qp.Price.ToString("C"))));
                    });

            context.For<Address>("Address")
                .Token("Honorific", a => a.Honorific)
                .Chain("Honorific", "Text", a => a.Honorific)
                .Token("FirstName", a => a.FirstName)
                .Chain("FirstName", "Text", a => a.FirstName)
                .Token("LastName", a => a.LastName)
                .Chain("LastName", "Text", a => a.LastName)
                .Token("Address1", a => a.Address1)
                .Chain("Address1", "Text", a => a.Address1)
                .Token("Address2", a => a.Address2)
                .Chain("Address2", "Text", a => a.Address2)
                .Token("Company", a => a.Company)
                .Chain("Company", "Text", a => a.Company)
                .Token("City", a => a.City)
                .Chain("City", "Text", a => a.City)
                .Token("Province", a => a.Province)
                .Chain("Province", "Text", a => a.Province)
                .Token("PostalCode", a => a.PostalCode)
                .Chain("PostalCode", "Text", a => a.PostalCode)
                .Token("Country", a => a.Country)
                .Chain("Country", "Text", a => a.Country);

            context.For<CheckoutError>("CheckoutError")
                .Token("Type", o => o.Type ?? "")
                .Chain("Type", "Text", o => o.Type ?? "")
                .Token("Message", o => o.Message ?? "")
                .Chain("Message", "Text", o => o.Message ?? "")
                .Token("Code", o => o.Code ?? "")
                .Chain("Code", "Text", o => o.Code ?? "");
        }
    }
}