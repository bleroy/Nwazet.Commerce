using System.Linq;
using System.Web.Mvc;
using Nwazet.Commerce.Permissions;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;
using Orchard;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;

namespace Nwazet.Commerce.Controllers {
    [Admin]
    [OrchardFeature("Nwazet.Attributes")]
    public class AttributesAdminController : Controller {
        private dynamic Shape { get; set; }
        private readonly ISiteService _siteService;
        private readonly IOrchardServices _orchardServices;
        private readonly IProductAttributeNameService _productAttributeNameService;
        private readonly IProductAttributeAdminServices _productAttributeAdminServices;

        public AttributesAdminController(
            ISiteService siteService,
            IShapeFactory shapeFactory,
            IOrchardServices orchardServices, 
            IProductAttributeNameService productAttributeNameService,
            IProductAttributeAdminServices productAttributeAdminServices) {
            
            _siteService = siteService;
            _orchardServices = orchardServices;
            _productAttributeNameService = productAttributeNameService;
            _productAttributeAdminServices = productAttributeAdminServices;

            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }


        public Localizer T { get; set; }
        
        public ActionResult Index(PagerParameters pagerParameters) {
            if (!_orchardServices.Authorizer.Authorize(CommercePermissions.ManageAttributes, null, T("Not authorized to manage product attributes"))) 
                return new HttpUnauthorizedResult();

            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);
            var attributes = _productAttributeAdminServices.GetAllParts();
            var paginatedAttributes = attributes
                .Skip(pager.GetStartIndex());
            if (pager.PageSize > 0) {
                paginatedAttributes = paginatedAttributes.Take(pager.PageSize);
            }
            var pageOfAttributes = paginatedAttributes.ToList();
            var pagerShape = Shape.Pager(pager).TotalItemCount(attributes.Count());
            var vm = new AttributesIndexViewModel {
                Attributes = pageOfAttributes,
                Pager = pagerShape
            };

            return View(vm);
        }

        public ActionResult AttributeName(string displayName, int version) {
            return Json(new {
                result = _productAttributeNameService.GenerateAttributeTechnicalName(displayName),
                version = version
            });
        }        
    }
}
