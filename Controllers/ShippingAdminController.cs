using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.Permissions;
using Nwazet.Commerce.ViewModels;
using Orchard;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;

namespace Nwazet.Commerce.Controllers {
    [Admin]
    [OrchardFeature("Nwazet.Shipping")]
    public class ShippingAdminController : Controller {
        private dynamic Shape { get; set; }
        private readonly ISiteService _siteService;
        private readonly IEnumerable<IShippingMethodProvider> _shippingMethodProviders;
        private readonly IOrchardServices _orchardServices;

        public ShippingAdminController(
            IEnumerable<IShippingMethodProvider> shippingMethodProviders,
            IShapeFactory shapeFactory,
            ISiteService siteService,
            IOrchardServices orchardServices) {

            _shippingMethodProviders = shippingMethodProviders;
            Shape = shapeFactory;
            _siteService = siteService;
            _orchardServices = orchardServices;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public ActionResult Index(PagerParameters pagerParameters) {
            if (!_orchardServices.Authorizer.Authorize(CommercePermissions.ManageShipping, null, T("Not authorized to manage shippings")))
                return new HttpUnauthorizedResult();

            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);
            var shippingMethods = _shippingMethodProviders
                .SelectMany(smp => smp.GetShippingMethods())
                .ToList();
            var paginatedMethods = shippingMethods
                .OrderBy(sm => sm.Name)
                .Skip(pager.GetStartIndex())
                .Take(pager.PageSize)
                .ToList();
            var pagerShape = Shape.Pager(pager).TotalItemCount(shippingMethods.Count());
            var vm = new ShippingMethodIndexViewModel {
                ShippingMethodProviders = _shippingMethodProviders.ToList(),
                ShippingMethods = paginatedMethods,
                Pager = pagerShape
            };

            return View(vm);
        }
    }
}
