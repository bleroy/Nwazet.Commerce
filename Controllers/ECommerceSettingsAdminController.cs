using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Nwazet.Commerce.Permissions;
using Orchard;
using Orchard.Localization;

namespace Nwazet.Commerce.Controllers {
    class ECommerceSettingsAdminController : Controller {

        private readonly IOrchardServices _orchardServices;

        public ECommerceSettingsAdminController(IOrchardServices orchardServices) {
            _orchardServices = orchardServices;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public ActionResult Index() {
            if (!_orchardServices.Authorizer.Authorize(CommercePermissions.ManageCommerce, null, T("Not authorized to manage e-commerce settings")))
                return new HttpUnauthorizedResult();

            return View();
        }

        [HttpPost, ActionName("Index")]
        public ActionResult IndexPost() {

            return RedirectToAction("Index");
        }
    }
}
