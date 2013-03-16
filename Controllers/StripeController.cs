using System.Web.Mvc;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;
using Orchard.Themes;

namespace Nwazet.Commerce.Controllers {
    [Themed]
    public class StripeController : Controller {
        private readonly IStripeService _stripeService;

        public StripeController(IStripeService stripeService) {
            _stripeService = stripeService;
        }

        public ActionResult Ship(string stripeToken) {
            var shippingViewModel = new StripeShippingViewModel {
                Token = stripeToken,
            };
            return View(shippingViewModel);
        }

        [HttpPost]
        public ActionResult Checkout() {
            var viewModel = new StripeCheckoutViewModel {
                PublishableKey = _stripeService.GetSettings().PublishableKey
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Confirm(string stripeToken,
                                    Address shippingAddress,
                                    Address billingAddress,
                                    string specialInstructions) {
            if (string.IsNullOrWhiteSpace(stripeToken)) {
                return RedirectToAction("Checkout");
            }
            return View();
        }
    }
}
