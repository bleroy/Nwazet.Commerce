using System.Web.Mvc;
using Nwazet.Commerce.Permissions;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Localization.Services;
using Orchard.Security;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Notify;

namespace Nwazet.Commerce.Controllers {
    [OrchardFeature("Nwazet.TieredPricing")]
    [ValidateInput(false), Admin]
    public class ProductSettingsAdminController : Controller, IUpdateModel {
        private readonly ISiteService _siteService;
        private readonly ICultureManager _cultureManager;
        private const string groupInfoId = "Pricing";
        public IOrchardServices Services { get; private set; }

        public ProductSettingsAdminController(
            ISiteService siteService,
            IOrchardServices services,
            ICultureManager cultureManager) {
            _siteService = siteService;
            _cultureManager = cultureManager;
            Services = services;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public ActionResult Index() {
            if (!Services.Authorizer.Authorize(CommercePermissions.ManagePricing, T("Not authorized to manage price settings")))
                return new HttpUnauthorizedResult();

            var site = _siteService.GetSiteSettings();
            dynamic model = Services.ContentManager.BuildEditor(site, groupInfoId);

            if (model == null)
                return HttpNotFound();

            return View(model);
        }

        [HttpPost, ActionName("Index")]
        public ActionResult IndexPOST() {
            if (!Services.Authorizer.Authorize(CommercePermissions.ManagePricing, T("Not authorized to manage price settings")))
                return new HttpUnauthorizedResult();

            var site = _siteService.GetSiteSettings();
            var model = Services.ContentManager.UpdateEditor(site, this, groupInfoId);

            if (model == null) {
                Services.TransactionManager.Cancel();
                return HttpNotFound();
            }

            if (!ModelState.IsValid) {
                Services.TransactionManager.Cancel();

                return View(model);
            }

            Services.Notifier.Information(T("Price settings updated"));
            return RedirectToAction("Index");
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }
}
