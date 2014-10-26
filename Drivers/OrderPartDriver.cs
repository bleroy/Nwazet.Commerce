using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Permissions;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Workflows.Services;
using Orchard.Security;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Nwazet.Orders")]
    public class OrderPartDriver : ContentPartDriver<OrderPart> {
        private readonly IOrderService _orderService;
        private readonly IAddressFormatter _addressFormatter;
        private readonly IEnumerable<ICheckoutService> _checkoutServices;
        private readonly IWorkContextAccessor _wca;
        private readonly IOrchardServices _orchardServices;
        private readonly IWorkflowManager _workflowManager;
        private readonly IMembershipService _membershipService;
        private readonly IEnumerable<IProductAttributeExtensionProvider> _extensionProviders;

        public OrderPartDriver(
            IOrderService orderService,
            IAddressFormatter addressFormatter,
            IEnumerable<ICheckoutService> checkoutServices,
            IWorkContextAccessor wca,
            IOrchardServices orchardServices,
            IWorkflowManager workflowManager,
            IMembershipService membershipService,
            IEnumerable<IProductAttributeExtensionProvider> extensionProviders) {

            _orderService = orderService;
            _addressFormatter = addressFormatter;
            _checkoutServices = checkoutServices;
            _wca = wca;
            _orchardServices = orchardServices;
            _workflowManager = workflowManager;
            _membershipService = membershipService;
            _extensionProviders = extensionProviders;
            T = NullLocalizer.Instance;
        }

        private const string ActivityName = "Activity";
        private const string BillingAddressName = "BillingAddress";
        private const string ChargeName = "Charge";
        private const string CardName = "Card";
        private const string EventName = "Event";
        private const string ItemName = "Item";
        private const string ItemsName = "Items";
        private const string ShippingAddressName = "ShippingAddress";
        private const string ShippingName = "Shipping";
        private const string TaxesName = "Taxes";
        private const string UserName = "User";

        protected override string Prefix {
            get { return "NwazetCommerceOrder"; }
        }

        public Localizer T { get; set; }

        protected override DriverResult Display(
            OrderPart part, string displayType, dynamic shapeHelper) {

            var orderShape = ContentShape(
                "Parts_Order",
                () => shapeHelper.Parts_Order(
                    Id: part.ContentItem.Id,
                    OrderedProducts: part.Items,
                    SubTotal: part.SubTotal,
                    Total: part.Total,
                    Taxes: part.Taxes,
                    AmountPaid: part.AmountPaid,
                    ShippingOption: part.ShippingOption,
                    BillingAddress: part.BillingAddress,
                    ShippingAddress: part.ShippingAddress,
                    CustomerEmail: part.CustomerEmail,
                    CustomerPhone: part.CustomerPhone,
                    IsTestOrder: part.IsTestOrder,
                    Password: part.Password,
                    SpecialInstructions: part.SpecialInstructions,
                    Status: part.Status,
                    StatusLabels: _orderService.StatusLabels,
                    TrackingUrl: part.TrackingUrl,
                    Charge: part.Charge,
                    Activity: part.Activity,

                    ContentPart: part
                    )
                );
            return orderShape;
        }

        //GET
        protected override DriverResult Editor(OrderPart part, dynamic shapeHelper) {
            if (!_orchardServices.Authorizer.Authorize(OrderPermissions.ManageOrders, null, T("Cannot manage orders")))
                return null;

            var contentManager = part.ContentItem.ContentManager;
            var productContents = contentManager
                .GetMany<IContent>(
                    part.Items.Select(p => p.ProductId).Distinct(),
                    VersionOptions.Latest, QueryHints.Empty);
            var products = productContents
                .ToDictionary(p => p.Id, p => p);
            var linkToTransaction = _checkoutServices
                .Select(s => s.GetChargeAdminUrl(part.Charge.TransactionId))
                .FirstOrDefault(u => u != null);
            var orderItems = part.Items.ToList();
            // Add attribute extension provider instances to order item attributes
            foreach (var item in orderItems) {
                if (item.Attributes != null) {
                    foreach (var attr in item.Attributes) {
                        attr.Value.ExtensionProviderInstance = _extensionProviders.SingleOrDefault(e => e.Name == attr.Value.ExtensionProvider);
                    }
                }
            }
            var model = new OrderEditorViewModel {
                Order = part,
                OrderItems = orderItems,
                Products = products,
                BillingAddressText = _addressFormatter.Format(part.BillingAddress),
                ShippingAddressText = _addressFormatter.Format(part.ShippingAddress),
                OrderStates = OrderPart.States,
                StatusLabels = _orderService.StatusLabels,
                EventCategories = OrderPart.EventCategories,
                EventCategoryLabels = _orderService.EventCategoryLabels,
                LinkToTransaction = linkToTransaction,
                UserName = part.User == null ? "" : part.User.UserName,
                UserNameNeeded = productContents.Any(p => p.As<ProductPart>() == null ? false : p.As<ProductPart>().AuthenticationRequired)
            };
            return ContentShape("Parts_Order_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/Order",
                    Model: model,
                    Prefix: Prefix));
        }

        //POST
        protected override DriverResult Editor(
            OrderPart part, IUpdateModel updater, dynamic shapeHelper) {

            if (!_orchardServices.Authorizer.Authorize(OrderPermissions.ManageOrders, null, T("Cannot manage orders")))
                return null;

            var previousTrackingUrl = part.TrackingUrl;
            var previousStatus = part.Status;
            var eventText = "";

            var updateModel = new OrderEditorViewModel {Order = part};
            updater.TryUpdateModel(updateModel, Prefix, null, null);

            var currentUser = _wca.GetContext().CurrentUser;
            if (currentUser != null) {
                eventText += T("Order updated by {0}. ", currentUser.UserName).Text;
            }

            if (previousTrackingUrl != part.TrackingUrl) {
                if (String.IsNullOrWhiteSpace(previousTrackingUrl)) {
                    eventText += T("Tracking URL changed to {0}. ", part.TrackingUrl).Text;
                }
                else {
                    eventText += T("Tracking URL changed from {0} to {1}. ", previousTrackingUrl, part.TrackingUrl).Text;
                }
                _workflowManager.TriggerEvent("OrderTrackingUrlChanged", part,
                    () => new Dictionary<string, object> {
                        {"Content", part},
                        {"Order", part}
                    });
            }

            if (previousStatus != part.Status) {
                eventText += T("Status changed from {0} to {1}. ",
                        _orderService.StatusLabels[previousStatus],
                        _orderService.StatusLabels[part.Status]).Text;
                _workflowManager.TriggerEvent("OrderStatusChanged", part,
                    () => new Dictionary<string, object> {
                        {"Content", part},
                        {"Order", part}
                    });

                foreach (var item in part.Items) {
                    var content = _orchardServices.ContentManager.Get(item.ProductId);
                    if (content != null) {
                        _workflowManager.TriggerEvent("OrderStatusChangedProduct", content,
                            () => new Dictionary<string, object> {
                                {"Content", content},
                                {"Order", part}
                            });
                    }
                }
            }
            
            if (!String.IsNullOrEmpty(updateModel.UserName)) {
                var user = _membershipService.GetUser(updateModel.UserName);
                if (user != null) {
                    part.UserId = user.Id;
                }
                else {
                    eventText += T("Provided username does not exist. ");
                }
            }

            if (!String.IsNullOrWhiteSpace(eventText)) {
                part.LogActivity(OrderPart.Event, eventText);
            }

            return Editor(part, shapeHelper);
        }

        protected override void Importing(OrderPart part, ImportContentContext context) {

            var xel = context.Data.Element(typeof (OrderPart).Name);
            if (xel == null) return;

            var itemsEl = xel.Element(ItemsName);

            var taxEl = xel.Element(TaxesName);
            var tax = new TaxAmount();
            if (taxEl != null) {
                taxEl.With(tax)
                    .FromAttr(t => t.Name)
                    .FromAttr(t => t.Amount);
            }

            var shippingEl = xel.Element(ShippingName);
            var shipping = new ShippingOption();
            if (shippingEl != null) {
                shippingEl.With(shipping)
                    .FromAttr(s => s.Description)
                    .FromAttr(s => s.ShippingCompany)
                    .FromAttr(s => s.Price);
            }

            var cardEl = xel.Element(CardName);
            var card = new CreditCardCharge();
            if (cardEl != null) {
                cardEl.With(card)
                    .FromAttr(c => c.TransactionId)
                    .FromAttr(c => c.Last4)
                    .FromAttr(c => c.ExpirationMonth)
                    .FromAttr(c => c.ExpirationYear);
            }

            var chargeEl = xel.Element(ChargeName);
            var charge = new Charge();
            if (chargeEl != null) {
                chargeEl.With(charge)
                    .FromAttr(c => c.TransactionId)
                    .FromAttr(c => c.ChargeText);
            }

            var el = xel.With(part);
            part.Build(
                card,
                itemsEl == null
                    ? null
                    : itemsEl.Elements(ItemName)
                        .Select(i => i.With(new CheckoutItem())
                            .FromAttr(coi => coi.ProductId)
                            .FromAttr(coi => coi.Title)
                            .FromAttr(coi => coi.Quantity)
                            .FromAttr(coi => coi.Price)
                            .FromAttr(coi => coi.LinePriceAdjustment)
                            .FromAttr(coi => coi.PromotionId)
                            .Context),
                el.Attr(p => p.SubTotal),
                el.Attr(p => p.Total),
                tax,
                shipping,
                Address.Get(xel.Element(ShippingAddressName)),
                Address.Get(xel.Element(BillingAddressName)),
                el.Attr(p => p.CustomerEmail),
                el.Attr(p => p.CustomerPhone),
                el.Attr(p => p.SpecialInstructions),
                el.Attr(p => p.AmountPaid));
            el.With(part)
                .FromAttr(p => p.IsTestOrder)
                .FromAttr(p => p.Password)
                .FromAttr(p => p.Status)
                .FromAttr(p => p.TrackingUrl)
                .FromAttr(p => p.PurchaseOrder);

            var activityEl = xel.Element(ActivityName);
            if (activityEl != null) {
                part.Activity = activityEl.Elements(EventName)
                    .Select(e => e.With(new OrderEvent())
                        .FromAttr(ev => ev.Date)
                        .FromAttr(ev => ev.Category)
                        .FromAttr(ev => ev.Description)
                        .Context);
            }

            var userNameEl = xel.Element(UserName);
            if (userNameEl != null) {
                var userName = userNameEl.Attr("UserName");
                if (!String.IsNullOrEmpty(userName)) {
                    part.User = _membershipService.GetUser(userName);
                }                
            }
        }

        protected override void Exporting(OrderPart part, ExportContentContext context) {
            context.Element(typeof (OrderPart).Name).With(part)
                .ToAttr(o => o.CustomerEmail)
                .ToAttr(o => o.CustomerPhone)
                .ToAttr(o => o.IsTestOrder)
                .ToAttr(o => o.Password)
                .ToAttr(o => o.SpecialInstructions)
                .ToAttr(o => o.Status)
                .ToAttr(o => o.SubTotal)
                .ToAttr(o => o.Total)
                .ToAttr(o => o.AmountPaid)
                .ToAttr(o => o.TrackingUrl)
                .ToAttr(o => o.PurchaseOrder)
                .Element

                .AddEl(new XElement(ItemsName,
                    part.Items == null ? new List<XElement>() : part.Items.Select(it =>
                        new XElement(ItemName).With(it)
                            .ToAttr(i => i.ProductId)
                            .ToAttr(i => i.Title)
                            .ToAttr(i => i.Quantity)
                            .ToAttr(i => i.Price)
                            .ToAttr(i => i.LinePriceAdjustment)
                            .ToAttr(i => i.PromotionId)
                            .Element)))

                .AddEl(new XElement(ShippingName).With(part.ShippingOption)
                    .ToAttr(s => s.Description)
                    .ToAttr(s => s.ShippingCompany)
                    .ToAttr(s => s.Price).Element)

                .AddEl(new XElement(TaxesName).With(part.Taxes)
                    .ToAttr(t => t.Name)
                    .ToAttr(t => t.Amount).Element)

                .AddEl(Address.Set(
                    new XElement(ShippingAddressName),
                    part.ShippingAddress))

                .AddEl(Address.Set(
                    new XElement(BillingAddressName),
                    part.BillingAddress))

                .AddEl(new XElement(ActivityName,
                    part.Activity == null ? new List<XElement>() : part.Activity.Select(ev => new XElement(EventName).With(ev)
                        .ToAttr(e => e.Date)
                        .ToAttr(e => e.Category)
                        .ToAttr(e => e.Description)
                        .Element)))

                .AddEl(new XElement(UserName).With(part.User)
                    .ToAttr(u => u.UserName)
                );
        }
    }
}