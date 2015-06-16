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
using Orchard.Settings;
using Orchard.Security;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;

namespace Nwazet.Commerce.Controllers {
    [Admin]
    [OrchardFeature("Nwazet.Promotions")]
    public class PromotionAdminController : Controller {
        private dynamic Shape { get; set; }
        private readonly ISiteService _siteService;
        private readonly IEnumerable<IPriceProvider> _priceProviders;
        private readonly IOrchardServices _orchardServices;

        public PromotionAdminController(
            IEnumerable<IPriceProvider> priceProviders,
            IShapeFactory shapeFactory,
            ISiteService siteService,
            IOrchardServices orchardServices) {

            _priceProviders = priceProviders;
            Shape = shapeFactory;
            _siteService = siteService;
            _orchardServices = orchardServices;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public ActionResult Index(PagerParameters pagerParameters) {
            if (!_orchardServices.Authorizer.Authorize(CommercePermissions.CommerceAdministrator, null, T("Not authorized to manage promotions")))
                return new HttpUnauthorizedResult();

            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);
            var promotions = _priceProviders
                .SelectMany(p => p.GetPromotions())
                .OrderBy(p => p.Name)
                .ToList();
            var paginatedPromotions = promotions
                .Skip(pager.GetStartIndex())
                .Take(pager.PageSize)
                .ToList();
            var pagerShape = Shape.Pager(pager).TotalItemCount(promotions.Count());
            var vm = new PromotionIndexViewModel {
                PriceProviders = _priceProviders.ToList(),
                Promotions = paginatedPromotions,
                Pager = pagerShape
            };

            return View(vm);
        }
    }
}
