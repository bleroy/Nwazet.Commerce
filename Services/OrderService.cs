using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Web.Mvc;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Mvc.Html;

namespace Nwazet.Commerce.Services {
    public class OrderService : IOrderService {
        private static readonly RNGCryptoServiceProvider RngCsp = new RNGCryptoServiceProvider();

        private readonly IContentManager _contentManager;
        private readonly UrlHelper _url;

        public OrderService(IContentManager contentManager, UrlHelper url) {
            _contentManager = contentManager;
            _url = url;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public string GetDisplayUrl(OrderPart order) {
            return _url.Action("Show", "OrderSsl", new {id = order.Id});
        }

        public string GetEditUrl(OrderPart order) {
            return _url.ItemEditUrl(order);
        }

        public OrderPart CreateOrder(
            CreditCardCharge creditCardCharge,
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
            string status,
            string trackingUrl = null,
            bool isTestOrder = false) {

            var order = _contentManager.Create("Order").As<OrderPart>();
            order.Build(creditCardCharge, items, subTotal, total, taxes,
                shippingOption, shippingAddress, billingAddress, customerEmail,
                customerPhone, specialInstructions);
            order.Status = status;
            order.TrackingUrl = trackingUrl;
            order.IsTestOrder = isTestOrder;

            var random = new byte[8];
            RngCsp.GetBytes(random);
            order.Password = Convert.ToBase64String(random);

            return order;
        }

        public OrderPart Get(int orderId) {
            return _contentManager.Get<OrderPart>(orderId);
        }

        public IDictionary<string, LocalizedString> StatusLabels {
            get {
                return new Dictionary<string, LocalizedString> {
                    {OrderPart.Pending, T("Pending")},
                    {OrderPart.Accepted, T("Accepted")},
                    {OrderPart.Archived, T("Archived")},
                    {OrderPart.Cancelled, T("Cancelled")}
                };
            }
        }

        public IDictionary<string, LocalizedString> EventCategoryLabels {
            get {
                return new Dictionary<string, LocalizedString> {
                    {OrderPart.Note, T("Note")},
                    {OrderPart.Warning, T("Warning")},
                    {OrderPart.Error, T("Error")},
                    {OrderPart.Task, T("Task")},
                    {OrderPart.Event, T("Event")}
                };
            }
        }
    }
}
