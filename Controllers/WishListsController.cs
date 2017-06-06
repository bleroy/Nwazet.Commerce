using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;
using Orchard;
using Orchard.Environment.Extensions;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nwazet.Commerce.Controllers {
    [OrchardFeature("Nwazet.WishLists")]
    [Themed]
    public class WishListsController : Controller {
        private readonly IWorkContextAccessor _wca;
        private readonly IWishListServices _wishListServices;

        public WishListsController(
            IWorkContextAccessor wca,
            IWishListServices wishListServices) {

            _wca = wca;
            _wishListServices = wishListServices;
        }

        public ActionResult Index(int id = 0) {
            var user = _wca.GetContext().CurrentUser;
            if (user == null) {
                return RedirectToAction("LogOn", "Account", new { area = "Orchard.Users" });
            }
            var wishLists = _wishListServices.GetWishLists(user);
            var selectedList = wishLists.SingleOrDefault(wp => wp.ContentItem.Id == id);
            if (selectedList == null) {
                selectedList = wishLists.SingleOrDefault(wp => wp.IsDefault);
            }
            return View(new WishListsIndexViewModel {
                WishLists = wishLists,
                WishList = selectedList
            });
        }
    }
}
