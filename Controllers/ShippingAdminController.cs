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
    [OrchardFeature("Nwazet.Shipping")]
    public class ShippingAdminController : Controller {
        private dynamic Shape { get; set; }
        private readonly ISiteService _siteService;
        private readonly IEnumerable<IShippingMethodProvider> _shippingMethodProviders;

        public ShippingAdminController(
            IEnumerable<IShippingMethodProvider> shippingMethodProviders,
            IShapeFactory shapeFactory,
            ISiteService siteService) {

            _shippingMethodProviders = shippingMethodProviders;
            Shape = shapeFactory;
            _siteService = siteService;
        }
        
        public ActionResult Index(PagerParameters pagerParameters) {
            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);
            var shippingMethods = _shippingMethodProviders
                .SelectMany(smp => smp.GetShippingMethods());
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
