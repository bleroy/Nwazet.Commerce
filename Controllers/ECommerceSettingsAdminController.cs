using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Nwazet.Commerce.Permissions;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Notify;

namespace Nwazet.Commerce.Controllers {
    [ValidateInput(false), Admin]
    public class ECommerceSettingsAdminController : Controller, IUpdateModel {

        private readonly IOrchardServices _orchardServices;
        private readonly ISiteService _siteService;
        private readonly ICurrencyProvider _currencyProvider;

        private const string groupInfoId = "ECommerceSiteSettings";

        public ECommerceSettingsAdminController(IOrchardServices orchardServices,
            ISiteService siteService,
            ICurrencyProvider currencyProvider) {

            _orchardServices = orchardServices;
            _siteService = siteService;
            _currencyProvider = currencyProvider;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public ActionResult Index() {
            if (!_orchardServices.Authorizer.Authorize(CommercePermissions.ManageCommerce, null, T("Not authorized to manage e-commerce settings")))
                return new HttpUnauthorizedResult();
            
            var site = _siteService.GetSiteSettings();
            dynamic model = _orchardServices.ContentManager.BuildEditor(site, groupInfoId);

            if (model == null)
                return HttpNotFound();


            return View(new ECommerceSettingsViewModel() {
                Model = model,
                CurrencyProvider = _currencyProvider
            });
        }

        [HttpPost, ActionName("Index")]
        public ActionResult IndexPost() {
            if (!_orchardServices.Authorizer.Authorize(CommercePermissions.ManageCommerce, null, T("Not authorized to manage e-commerce settings")))
                return new HttpUnauthorizedResult();

            var site = _siteService.GetSiteSettings();
            var model = _orchardServices.ContentManager.UpdateEditor(site, this, groupInfoId);

            if (model == null) {
                _orchardServices.TransactionManager.Cancel();
                return HttpNotFound();
            }

            if (!ModelState.IsValid) {
                _orchardServices.TransactionManager.Cancel();

                return View(model);
            }
            _orchardServices.Notifier.Information(T("Store settings updated"));

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
