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
    [OrchardFeature("Nwazet.Taxes")]
    public class TaxAdminController : Controller {
        private dynamic Shape { get; set; }
        private readonly ISiteService _siteService;
        private readonly IEnumerable<ITaxProvider> _taxProviders;

        public TaxAdminController(
            IEnumerable<ITaxProvider> taxProviders,
            IShapeFactory shapeFactory,
            ISiteService siteService) {

            _taxProviders = taxProviders;
            Shape = shapeFactory;
            _siteService = siteService;
        }
        
        public ActionResult Index(PagerParameters pagerParameters) {
            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);
            var taxes = _taxProviders
                .SelectMany(tp => tp.GetTaxes())
                .ToList();
            var paginatedTaxes = taxes
                .OrderBy(t => t.Name)
                .Skip(pager.GetStartIndex())
                .Take(pager.PageSize)
                .ToList();
            var pagerShape = Shape.Pager(pager).TotalItemCount(taxes.Count());
            var vm = new TaxIndexViewModel {
                TaxProviders = _taxProviders.ToList(),
                Taxes = paginatedTaxes,
                Pager = pagerShape
            };

            return View(vm);
        }
    }
}
