using System;
using System.Linq;
using System.Web.Mvc;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Mvc;
using Orchard.Themes;
using Orchard.UI.Notify;

namespace Nwazet.Commerce.Controllers {
    [Themed]
    [OrchardFeature("Nwazet.Orders")]
    public class OrderSslController : Controller {
        private readonly IOrderService _orderService;
        private readonly IContentManager _contentManager;
        private readonly IWorkContextAccessor _wca;
        private readonly dynamic _shapeFactory;
        private readonly IAddressFormatter _addressFormatter;
        private readonly INotifier _notifier;
        private readonly IShoppingCart _shoppingCart;

        public OrderSslController(
            IOrderService orderService,
            IContentManager contentManager,
            IWorkContextAccessor wca,
            IShapeFactory shapeFactory,
            IAddressFormatter addressFormatter,
            INotifier notifier,
            IShoppingCart shoppingCart) {

            _orderService = orderService;
            _contentManager = contentManager;
            _wca = wca;
            _shapeFactory = shapeFactory;
            _addressFormatter = addressFormatter;
            _notifier = notifier;
            _shoppingCart = shoppingCart;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Confirmation() {
            if (!TempData.ContainsKey("OrderId")) {
                return HttpNotFound();
            }
            var orderId = TempData["OrderId"];
            var order = _contentManager.Get<OrderPart>((int) orderId);
            var billingAddress = _addressFormatter.Format(order.BillingAddress);
            var shippingAddress = _addressFormatter.Format(order.ShippingAddress);
            var products = _contentManager
                .GetMany<IContent>(
                    order.Items.Select(p => p.ProductId).Distinct(),
                    VersionOptions.Latest,
                    QueryHints.Empty)
                .ToDictionary(p => p.Id, p => p);
            var shape = _shapeFactory.Order_Confirmation(
                OrderId: order.Id,
                Status: _orderService.StatusLabels[order.Status],
                Items: order.Items,
                Products: products,
                SubTotal: order.SubTotal,
                Taxes: order.Taxes,
                Total: order.Total,
                ShippingOption: order.ShippingOption,
                BillingAddress: billingAddress,
                ShippingAddress: shippingAddress,
                TrackingUrl: order.TrackingUrl,
                CustomerEmail: order.CustomerEmail,
                CustomerPhone: order.CustomerPhone,
                CardLast4: order.CreditCardCharge.Last4,
                CardExpirationMonth: order.CreditCardCharge.ExpirationMonth,
                CardExpirationYear: order.CreditCardCharge.ExpirationYear,
                SpecialInstructions: order.SpecialInstructions,
                BaseUrl: _wca.GetContext().CurrentSite.BaseUrl,
                Password: order.Password);
            _shoppingCart.Clear();
            return new ShapeResult(this, shape);
        }

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Show(int id, string password = null) {
            if (TempData.ContainsKey("OrderId")) {
                return Confirmation();
            }

            if (!String.IsNullOrWhiteSpace(password)) {
                var order = _contentManager.Get<OrderPart>(id);
                if (!password.EndsWith("=")) {
                    password += "=";
                }
                if (password != order.Password) {
                    _notifier.Error(T("Wrong password"));
                }
                else {
                    TempData["OrderId"] = id;
                    return Confirmation();
                }
            }
            return new ShapeResult(this, _shapeFactory.Order_CheckPassword(
                OrderId: id));
        }
    }
}