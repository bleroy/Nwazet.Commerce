using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Environment.Extensions;
using Orchard.Security;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Orders")]
    public class OrderPart : ContentPart<OrderPartRecord>, ITitleAspect {
        public const string Pending = "Pending";
        public const string Accepted = "Accepted";
        public const string Archived = "Archived";
        public const string Cancelled = "Cancelled";

        public static readonly string[] States = {
            Pending, Accepted, Archived, Cancelled
        };

        public const string Note = "Note";
        public const string Warning = "Warning";
        public const string Error = "Error";
        public const string Task = "Task";
        public const string Event = "Event";

        public static readonly string[] EventCategories = {
            Note, Warning, Error, Task, Event
        };

        private XElement _contentDocument;
        private XElement _customerDocument;
        private XElement _activityDocument;

        private const string ActivityName = "activity";
        private const string BillingAddressName = "billingAddress";
        private const string ChargeName = "charge"; 
        private const string CardName = "card";
        private const string ContentName = "content";
        private const string CustomerName = "customer";
        private const string EmailName = "email";
        private const string EventName = "event";
        private const string EventsName = "events";
        private const string InstructionsName = "instructions";
        private const string ItemName = "item";
        private const string ItemsName = "items";
        private const string AttributesName = "attributes";
        private const string AttributeName = "attribute";
        private const string PhoneName = "phone";
        private const string ShippingAddressName = "shippingAddress";
        private const string ShippingName = "shipping";
        private const string SubtotalName = "subtotal";
        private const string TaxesName = "taxes";
        private const string TotalName = "total";
        private const string AmountName = "amountPaid";
        private const string PurchaseOrderName = "purchaseOrder";

        public void Build(
            ICharge charge,
            IEnumerable<CheckoutItem> items,
            double subTotal,
            double total,
            TaxAmount taxes,
            ShippingOption shippingOption,
            Address shippingAddress,
            Address billingAddress,
            string customerEmail,
            string customerPhone,
            string specialInstructions, 
            double amountPaid = default(double),
            string purchaseOrder = "") {

            _contentDocument = new XElement(ContentName)
                .Attr(SubtotalName, subTotal)
                .Attr(TotalName, total)
                .Attr(AmountName, amountPaid == default(double) ? total : amountPaid)
                .Attr(PurchaseOrderName, purchaseOrder)
                .AddEl(new XElement(ChargeName).With(charge)
                    .ToAttr(c => c.TransactionId)
                    .ToAttr(c => c.ChargeText)
                    .Element)
                .AddEl(new XElement(ItemsName, items.Select(it =>
                    new XElement(ItemName).With(it)
                        .ToAttr(i => i.ProductId)
                        .ToAttr(i => i.Quantity)
                        .ToAttr(i => i.Title)
                        .ToAttr(i => i.Price)
                        .ToAttr(i => i.LinePriceAdjustment)
                        .ToAttr(i => i.PromotionId)
                        .Element
                    .AddEl(new XElement(AttributesName, it.Attributes != null ? it.Attributes.Select(at => {
                            var attrEl = new XElement(AttributeName);
                            attrEl.SetAttributeValue("Key", at.Key);
                            attrEl.SetAttributeValue("Value", at.Value.Value);
                            attrEl.SetAttributeValue("Extra", at.Value.ExtendedValue);
                            attrEl.SetAttributeValue("ExtensionProvider", at.Value.ExtensionProvider);
                            return attrEl;
                            }) : null)))))
                .AddEl(new XElement(TaxesName).With(taxes)
                    .ToAttr(t => t.Name)
                    .ToAttr(t => t.Amount)
                    .Element)
                .AddEl(new XElement(ShippingName).With(shippingOption)
                    .ToAttr(s => s.Description)
                    .ToAttr(s => s.ShippingCompany)
                    .ToAttr(s => s.Price)
                    .Element);

            var shippingAddressElement = Address.Set(new XElement(ShippingAddressName), shippingAddress);
            var billingAddressElement = Address.Set(new XElement(BillingAddressName), billingAddress);

            _customerDocument = new XElement(CustomerName)
                .AddEl(shippingAddressElement)
                .AddEl(billingAddressElement)
                .Attr(EmailName, customerEmail)
                .Attr(PhoneName, customerPhone)
                .Attr(InstructionsName, specialInstructions);

            PersistContents();
            PersistCustomer();
        }

        private void PersistContents() {
            Record.Contents = _contentDocument.ToString(SaveOptions.None);
        }

        private void PersistCustomer() {
            Record.Customer = CustomerDocument.ToString(SaveOptions.None);
        }

        public string Status {
            get { return Record.Status; }
            set { Record.Status = value; }
        }

        private XElement ContentDocument {
            get {
                if (_contentDocument != null) return _contentDocument;
                var content = Record.Contents;
                return _contentDocument = string.IsNullOrWhiteSpace(content)
                    ? new XElement(ContentName)
                    : XElement.Parse(content);
            }
        }

        public IEnumerable<CheckoutItem> Items {
            get {
                var itemsElement = ContentDocument
                    .Element(ItemsName);
                if (itemsElement == null) return new CheckoutItem[0];
                return itemsElement
                    .Elements(ItemName)
                    .Select(el => {
                        var checkoutItem = el.With(new CheckoutItem())
                        .FromAttr(i => i.ProductId)
                        .FromAttr(i => i.Quantity)
                        .FromAttr(i => i.Title)
                        .FromAttr(i => i.Price)
                        .FromAttr(i => i.LinePriceAdjustment)
                        .FromAttr(i => i.PromotionId)
                        .Context;
                        if (el.Element(AttributesName) != null) {
                            checkoutItem.Attributes =
                                el.Elements(AttributesName).Elements(AttributeName).Select(a =>
                                    new {
                                        Key = int.Parse(a.Attr("Key")),
                                        Value = new ProductAttributeValueExtended {
                                            Value = a.Attr("Value"),
                                            ExtendedValue = a.Attr("Extra"),
                                            ExtensionProvider = a.Attr("ExtensionProvider")
                                        }
                                    }).ToDictionary(k => k.Key, k => k.Value);
                        }
                        return checkoutItem;
                    });
            }
        }

        public double SubTotal {
            get {
                return (double) ContentDocument.Attribute(SubtotalName);
            }
        }

        public double Total {
            get {
                return (double) ContentDocument.Attribute(TotalName);
            }
        }

        public double AmountPaid {
            get {
                var attr = ContentDocument.Attribute(AmountName);
                if (attr == null) return Total;
                return (double) attr;
            }
            set {
                ContentDocument.SetAttributeValue(AmountName, value);
                PersistContents();
            }
        }

        public string PurchaseOrder {
            get { return ContentDocument.Attr(PurchaseOrderName); }
            set {
                ContentDocument.Attr(PurchaseOrderName, value ?? "");
                PersistContents();
            }
        }

        public ICharge Charge {
            get {
                var chargeElement = ContentDocument.Element(ChargeName);
                if (chargeElement == null) {
                    // Handle card specific data, prior to ICharge
                    var cardElement = ContentDocument.Element(CardName);
                    if (cardElement == null) return null;
                    return cardElement.With(new CreditCardCharge())
                        .FromAttr(c => c.TransactionId)
                        .FromAttr(c => c.Last4)
                        .FromAttr(c => c.ExpirationMonth)
                        .FromAttr(c => c.ExpirationYear)
                        .Context;
                }
                return chargeElement.With(new Charge())
                    .FromAttr(c => c.TransactionId)
                    .FromAttr(c => c.ChargeText)
                    .Context;
            }
        }

        public TaxAmount Taxes {
            get {
                var taxElement = ContentDocument.Element(TaxesName);
                if (taxElement == null) return null;
                return taxElement.With(new TaxAmount())
                    .FromAttr(t => t.Name)
                    .FromAttr(t => t.Amount)
                    .Context;
            }
        }

        public ShippingOption ShippingOption {
            get {
                var shippingElement = ContentDocument.Element(ShippingName);
                if (shippingElement == null) return null;
                return shippingElement.With(new ShippingOption())
                    .FromAttr(s => s.Description)
                    .FromAttr(s => s.ShippingCompany)
                    .FromAttr(s => s.Price)
                    .Context;
            }
        }

        private XElement CustomerDocument {
            get {
                if (_customerDocument != null) return _customerDocument;
                var customer = Record.Customer;
                return _customerDocument = string.IsNullOrWhiteSpace(customer)
                    ? new XElement(CustomerName)
                    : XElement.Parse(customer);
            }
        }

        public Address ShippingAddress {
            get {
                var shippingAddressElement = CustomerDocument.Element(ShippingAddressName);
                if (shippingAddressElement == null) return null;
                return Address.Get(shippingAddressElement);
            }
            set {
                var shippingAddressElement = CustomerDocument.Element(ShippingAddressName);
                if (shippingAddressElement == null) {
                    shippingAddressElement = new XElement(ShippingAddressName);
                    CustomerDocument.Add(shippingAddressElement);
                }
                Address.Set(shippingAddressElement, value);
                PersistCustomer();
            }
        }

        public Address BillingAddress {
            get {
                var billingAddressElement = CustomerDocument.Element(BillingAddressName);
                if (billingAddressElement == null) return null;
                return Address.Get(billingAddressElement);
            }
            set {
                var billingAddressElement = CustomerDocument.Element(BillingAddressName);
                if (billingAddressElement == null) {
                    billingAddressElement = new XElement(BillingAddressName);
                    CustomerDocument.Add(billingAddressElement);
                }
                Address.Set(billingAddressElement, value);
                PersistCustomer();
            }
        }

        public string CustomerEmail {
            get {
                return CustomerDocument.Attr(EmailName);
            }
        }

        public string CustomerPhone {
            get {
                return CustomerDocument.Attr(PhoneName);
            }
        }

        public string SpecialInstructions {
            get {
                return CustomerDocument.Attr(InstructionsName);
            }
        }

        private XElement ActivityDocument {
            get {
                if (_activityDocument != null) return _activityDocument;
                var activity = Record.Activity;
                return _activityDocument = string.IsNullOrWhiteSpace(activity)
                    ? new XElement(ActivityName)
                    : XElement.Parse(activity);
            }
        }

        public IEnumerable<OrderEvent> Activity {
            get {
                var eventsElement = ActivityDocument.Element(EventsName);
                if (eventsElement == null) return null;
                return eventsElement.Elements(EventName)
                    .Select(ev => ev.With(new OrderEvent())
                        .FromAttr(e => e.Date)
                        .FromAttr(e => e.Category)
                        .FromAttr(e => e.Description)
                        .Context);
            }
            internal set {
                _activityDocument =
                    new XElement(ActivityName,
                        new XElement(EventsName, 
                            value.Select(ev => new XElement(EventName).With(ev)
                                .ToAttr(e => e.Date)
                                .ToAttr(e => e.Category)
                                .ToAttr(e => e.Description)
                                .Element)));
            }
        }

        public OrderEvent LogActivity(string category, string description) {
            var eventsElement = ActivityDocument.Element(EventsName);
            if (eventsElement == null) {
                ActivityDocument.Add(eventsElement = new XElement(EventsName));
            }
            var orderEvent = new OrderEvent {
                Date = DateTime.UtcNow,
                Category = category,
                Description = description
            };
            eventsElement.Add(new XElement(EventName).With(orderEvent)
                .ToAttr(e => e.Date)
                .ToAttr(e => e.Category)
                .ToAttr(e => e.Description)
                .Element);
            Record.Activity = ActivityDocument.ToString(SaveOptions.DisableFormatting);
            return orderEvent;
        }

        public string TrackingUrl {
            get { return Record.TrackingUrl; }
            set { Record.TrackingUrl = value; }
        }

        public string Password {
            get { return Record.Password; }
            set { Record.Password = value; }
        }

        public bool IsTestOrder {
            get { return Record.IsTestOrder; }
            set { Record.IsTestOrder = value; }
        }

        public string Title {
            get { return Id + " - " +
                Status + " - " +
                BillingAddress.Honorific + " " + BillingAddress.FirstName + " " + BillingAddress.LastName + " - " +
                Total.ToString("C") +
                (IsTestOrder ? " - TEST" : ""); }
        }

        public int UserId {
            get { return Record.UserId; }
            set { Record.UserId = value; }
        }

        public IUser User {
            get {
                IUser user = null;
                if (this.ContentItem != null) {
                    if (this.ContentItem.ContentManager != null) {
                        user = this.ContentItem.ContentManager.Get<IUser>(UserId);
                    }
                }
                return user;
            }
            set { UserId = value == null ? -1 : value.Id; }
        }
    }
}
