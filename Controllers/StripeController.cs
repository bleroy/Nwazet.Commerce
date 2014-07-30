using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;
using Orchard;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Themes;
using Orchard.UI.Notify;
using Orchard.Workflows.Services;

namespace Nwazet.Commerce.Controllers {
    [Themed]
    [RequireHttps]
    [OrchardFeature("Stripe")]
    public class StripeController : Controller {
        private const string NwazetStripeCheckout = "nwazet.stripe.checkout";
        private readonly IStripeService _stripeService;
        private readonly IOrderService _orderService;
        private readonly IWorkContextAccessor _wca;
        private readonly IWorkflowManager _workflowManager;
        private readonly INotifier _notifier;

        public StripeController(
            IStripeService stripeService,
            IOrderService orderService,
            IWorkContextAccessor wca,
            IWorkflowManager workflowManager,
            INotifier notifier) {

            _stripeService = stripeService;
            _orderService = orderService;
            _wca = wca;
            _workflowManager = workflowManager;
            _notifier = notifier;

            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }

        [HttpPost]
        public ActionResult Checkout(string checkoutData) {
            var stripeData = _stripeService.DecryptCheckoutData(checkoutData);
            GetCheckoutData(stripeData);
            return RedirectToAction("Ship");
        }

        public ActionResult Ship() {
            _wca.GetContext().Layout.IsCartPage = true;
            var checkoutData = GetCheckoutData(new StripeCheckoutViewModel {
                Amount = 0
            });
            if (checkoutData.CheckoutItems == null || !checkoutData.CheckoutItems.Any()) {
                return RedirectToAction("Index", "ShoppingCart");
            }
            return View(checkoutData);
        }

        [HttpPost]
        public ActionResult Ship(StripeCheckoutViewModel stripeData, string next, string back) {
            if (!String.IsNullOrWhiteSpace(back)) {
                return RedirectToAction("Index", "ShoppingCart");
            }
            var checkoutData = GetCheckoutData(stripeData);
            if (AnyEmptyString(
                checkoutData.Email,
                checkoutData.BillingAddress.FirstName,
                checkoutData.BillingAddress.LastName,
                checkoutData.BillingAddress.Address1,
                checkoutData.BillingAddress.City,
                checkoutData.ShippingAddress.FirstName,
                checkoutData.ShippingAddress.LastName,
                checkoutData.ShippingAddress.Address1,
                checkoutData.ShippingAddress.City
                )) {
                return RedirectToAction("Ship");
            }
            return RedirectToAction("Pay");
        }

        public ActionResult Pay(string errorMessage = null) {
            _wca.GetContext().Layout.IsCartPage = true;
            var checkoutData = GetCheckoutData();
            if ((checkoutData.CheckoutItems == null || !checkoutData.CheckoutItems.Any()) && checkoutData.Amount <= 0) {
                return RedirectToAction("Index", "ShoppingCart");
            }
            checkoutData.PublishableKey = _stripeService.GetSettings().PublishableKey;
            if (!String.IsNullOrEmpty(errorMessage)) {
                _notifier.Error(new LocalizedString(errorMessage));
            }
            return View(checkoutData);
        }

        [HttpPost]
        public ActionResult Pay(StripeCheckoutViewModel stripeData, string stripeToken, string next, string back) {
            var checkoutData = GetCheckoutData(stripeData);
            if (!String.IsNullOrWhiteSpace(back)) {
                return RedirectToAction(checkoutData.Amount >= 0 ? "SendMoney" : "Ship");
            }
            var subTotal = 0.0;
            var total = checkoutData.Amount;
            var isProductOrder = checkoutData.CheckoutItems.Any();
            if (isProductOrder) {
                var taxes = checkoutData.Taxes == null ? 0 : checkoutData.Taxes.Amount;
                subTotal = checkoutData.CheckoutItems.Sum(i => i.Price*i.Quantity + i.LinePriceAdjustment);
                total = subTotal + taxes + checkoutData.ShippingOption.Price;
            }
            // Call Stripe to charge card
            var stripeCharge = _stripeService.Charge(stripeToken, total);

            if (stripeCharge.Error != null) {
                Logger.Error(stripeCharge.Error.Type + ": " + stripeCharge.Error.Message);
                _workflowManager.TriggerEvent("OrderError", null,
                    () => new Dictionary<string, object> {
                        {"CheckoutError", stripeCharge.Error}
                    });
                if (stripeCharge.Error.Type == "card_error") {
                    return Pay(stripeCharge.Error.Message);
                }
                throw new InvalidOperationException(stripeCharge.Error.Type + ": " + stripeCharge.Error.Message);
            }
            
            var userId = -1;
            var currentUser = _wca.GetContext().CurrentUser;
            if (currentUser != null) {
                userId = currentUser.Id;            
            }           

            var order = _orderService.CreateOrder(
                stripeCharge,
                checkoutData.CheckoutItems,
                subTotal,
                total,
                checkoutData.Taxes,
                checkoutData.ShippingOption,
                checkoutData.ShippingAddress,
                checkoutData.BillingAddress,
                checkoutData.Email,
                checkoutData.Phone,
                checkoutData.SpecialInstructions,
                OrderPart.Pending,
                null,
                _stripeService.IsInTestMode(),
                userId,
                total,
                checkoutData.PurchaseOrder);
            TempData["OrderId"] = order.Id;
            _workflowManager.TriggerEvent(
                isProductOrder ? "NewOrder" : "NewPayment",
                order,
                () => new Dictionary<string, object> {
                    {"Content", order},
                    {"Order", order}
                });
            order.LogActivity(OrderPart.Event, T("Order created.").Text);
            // Clear checkout info from temp data
            TempData.Remove(NwazetStripeCheckout);

            return RedirectToAction("Confirmation", "OrderSsl");
        }

        public ActionResult SendMoney(string purchaseOrder = "", double amount = 0) {
            _wca.GetContext().Layout.IsCartPage = true;
            var checkoutData = new StripeCheckoutViewModel {
                Amount = amount,
                PurchaseOrder = purchaseOrder,
                CheckoutItems = new CheckoutItem[] { }
            };
            checkoutData = GetCheckoutData(checkoutData);
            return View(checkoutData);
        }

        [HttpPost]
        public ActionResult SendMoney(StripeCheckoutViewModel stripeData, string next) {
            var checkoutData = GetCheckoutData(stripeData);
            if (AnyEmptyString(
                checkoutData.Email,
                checkoutData.BillingAddress.FirstName,
                checkoutData.BillingAddress.LastName,
                checkoutData.BillingAddress.Address1,
                checkoutData.BillingAddress.City)
                || checkoutData.Amount <= 0) {
                return RedirectToAction("SendMoney");
            }
            return RedirectToAction("Pay");
        }

        private StripeCheckoutViewModel GetCheckoutData(StripeCheckoutViewModel updateModel = null) {
            var checkoutData = TempData[NwazetStripeCheckout] as StripeCheckoutViewModel ??
                               new StripeCheckoutViewModel {
                                   CheckoutItems = new CheckoutItem[0],
                                   ShippingOption = new ShippingOption(),
                                   ShippingAddress = new Address(),
                                   BillingAddress = new Address()
                               };
            if (updateModel != null) {
                if (updateModel.CheckoutItems != null) {
                    checkoutData.CheckoutItems = updateModel.CheckoutItems;
                }
                if (updateModel.ShippingOption != null) {
                    checkoutData.ShippingOption = updateModel.ShippingOption;
                }
                if (updateModel.BillingAddress != null) {
                    checkoutData.BillingAddress = updateModel.BillingAddress;
                }
                if (updateModel.ShippingAddress != null) {
                    checkoutData.ShippingAddress = updateModel.ShippingAddress;
                }
                if (updateModel.Taxes != null) {
                    checkoutData.Taxes = updateModel.Taxes;
                }
                if (updateModel.Email != null) {
                    checkoutData.Email = updateModel.Email;
                }
                if (updateModel.Phone != null) {
                    checkoutData.Phone = updateModel.Phone;
                }
                if (updateModel.SpecialInstructions != null) {
                    checkoutData.SpecialInstructions = updateModel.SpecialInstructions;
                }
                if (updateModel.Token != null) {
                    checkoutData.Token = updateModel.Token;
                }
                if (!String.IsNullOrWhiteSpace(updateModel.PurchaseOrder)) {
                    checkoutData.PurchaseOrder = updateModel.PurchaseOrder;
                }
                if (updateModel.Amount > 0) {
                    checkoutData.Amount = updateModel.Amount;
                }
            }
            TempData[NwazetStripeCheckout] = checkoutData;
            TempData.Keep(NwazetStripeCheckout);
            return checkoutData;
        }

        private bool AnyEmptyString(params string[] strings) {
            return strings.Any(String.IsNullOrWhiteSpace);
        }
    }
}