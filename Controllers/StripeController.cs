using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;
using Orchard;
using Orchard.Themes;
using Orchard.Workflows.Services;

namespace Nwazet.Commerce.Controllers {
    [Themed]
    [RequireHttps]
    public class StripeController : Controller {
        private const string NwazetStripeCheckout = "nwazet.stripe.checkout";
        private readonly IStripeService _stripeService;
        private readonly IOrderService _orderService;
        private readonly IWorkContextAccessor _wca;
        private readonly IWorkflowManager _workflowManager;

        public StripeController(
            IStripeService stripeService,
            IOrderService orderService,
            IWorkContextAccessor wca,
            IWorkflowManager workflowManager) {

            _stripeService = stripeService;
            _orderService = orderService;
            _wca = wca;
            _workflowManager = workflowManager;
        }

        [HttpPost]
        public ActionResult Checkout(string checkoutData) {
            var stripeData = _stripeService.DecryptCheckoutData(checkoutData);
            GetCheckoutData(stripeData);
            return RedirectToAction("Ship");
        }

        public ActionResult Ship() {
            _wca.GetContext().Layout.IsCartPage = true;
            var checkoutData = GetCheckoutData();
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
            GetCheckoutData(stripeData);
            return RedirectToAction("Pay");
        }

        public ActionResult Pay() {
            _wca.GetContext().Layout.IsCartPage = true;
            var checkoutData = GetCheckoutData();
            if (checkoutData.CheckoutItems == null || !checkoutData.CheckoutItems.Any()) {
                return RedirectToAction("Index", "ShoppingCart");
            }
            checkoutData.PublishableKey = _stripeService.GetSettings().PublishableKey;
            return View(checkoutData);
        }

        [HttpPost]
        public ActionResult Pay(StripeCheckoutViewModel stripeData, string stripeToken, string next, string back) {
            var checkoutData = GetCheckoutData(stripeData);
            if (!String.IsNullOrWhiteSpace(back)) {
                return RedirectToAction("Ship");
            }
            var taxes = checkoutData.Taxes == null ? 0 : checkoutData.Taxes.Amount;
            var subTotal = checkoutData.CheckoutItems.Sum(i => i.Price*i.Quantity);
            var total = subTotal + taxes + checkoutData.ShippingOption.Price;
            // Call Stripe to charge card
            var stripeCharge = _stripeService.Charge(stripeToken, total);

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
                _stripeService.IsInTestMode());
            TempData["OrderId"] = order.Id;
            _workflowManager.TriggerEvent("NewOrder", order,
                () => new Dictionary<string, object> {
                    {"Content", order},
                    {"Order", order}
                });

            return RedirectToAction("Confirmation", "OrderSsl");
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
            }
            TempData[NwazetStripeCheckout] = checkoutData;
            TempData.Keep(NwazetStripeCheckout);
            return checkoutData;
        }
    }
}
