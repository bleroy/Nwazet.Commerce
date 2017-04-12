using System;
using System.Linq;
using System.Web.Mvc;
using Nwazet.Commerce.Extensions;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Permissions;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.Utility.Extensions;

namespace Nwazet.Commerce.Controllers {
    [Admin]
    [OrchardFeature("Nwazet.Attributes")]
    public class AttributesAdminController : Controller {
        private dynamic Shape { get; set; }
        private readonly ISiteService _siteService;
        private readonly IContentManager _contentManager;
        private readonly IOrchardServices _orchardServices;
        private readonly IProductAttributeNameService _productAttributeNameService;

        public AttributesAdminController(
            IContentManager contentManager,
            ISiteService siteService,
            IShapeFactory shapeFactory,
            IOrchardServices orchardServices, 
            IProductAttributeNameService productAttributeNameService) {

            _contentManager = contentManager;
            _siteService = siteService;
            _orchardServices = orchardServices;
            _productAttributeNameService = productAttributeNameService;

            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }


        public Localizer T { get; set; }
        
        public ActionResult Index(PagerParameters pagerParameters) {
            if (!_orchardServices.Authorizer.Authorize(CommercePermissions.ManageAttributes, null, T("Not authorized to manage product attributes"))) 
                return new HttpUnauthorizedResult();

            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);
            var attributes = _contentManager
                .Query<ProductAttributePart>()
                .Join<TitlePartRecord>()
                .OrderBy(p => p.Title)
                .List().ToList();
            var paginatedAttributes = attributes
                .Skip(pager.GetStartIndex())
                .Take(pager.PageSize)
                .ToList();
            var pagerShape = Shape.Pager(pager).TotalItemCount(attributes.Count());
            var vm = new AttributesIndexViewModel {
                Attributes = paginatedAttributes,
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
