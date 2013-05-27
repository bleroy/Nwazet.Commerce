using System.Web.Mvc;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;
using Orchard.Themes;

namespace Nwazet.Commerce.Controllers {
    [Themed]
    public class StripeController : Controller {
        private readonly IStripeService _stripeService;
        private readonly IShoppingCart _shoppingCart;

        public StripeController(IStripeService stripeService, IShoppingCart shoppingCart) {
            _stripeService = stripeService;
            _shoppingCart = shoppingCart;
        }

        public ActionResult Checkout() {
            return RedirectToAction("Ship");
        }

        public ActionResult Ship() {
            return View(GetCheckoutData());
        }

        [HttpPost]
        public ActionResult Ship(StripeCheckoutViewModel stripeData) {
            return RedirectToAction("Confirmation");
        }

        [HttpPost]
        public ActionResult Pay() {
            return View(GetCheckoutData());
        }

        [HttpPost]
        public ActionResult Confirmation(StripeCheckoutViewModel stripeData) {
            return View();
        }

        private StripeCheckoutViewModel GetCheckoutData() {
            var checkoutData = TempData["nwazet.stripe.checkout"] as StripeCheckoutViewModel;
            if (checkoutData == null) {
                TempData["nwazet.stripe.checkout"] = checkoutData =
                    new StripeCheckoutViewModel {
                    ShoppingCartItems = _shoppingCart.Items,
                    PublishableKey = _stripeService.GetSettings().PublishableKey
                };
            }
            return checkoutData;
        }
    }
}
