using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;

namespace Nwazet.Commerce.Controllers {
    [Admin]
    [OrchardFeature("Nwazet.Promotions")]
    public class PromotionAdminController : Controller {
        private dynamic Shape { get; set; }
        private readonly ISiteService _siteService;
        private readonly IEnumerable<IPriceProvider> _priceProviders;

        public PromotionAdminController(
            IEnumerable<IPriceProvider> priceProviders,
            IShapeFactory shapeFactory,
            ISiteService siteService) {

            _priceProviders = priceProviders;
            Shape = shapeFactory;
            _siteService = siteService;
        }
        
        public ActionResult Index(PagerParameters pagerParameters) {
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
