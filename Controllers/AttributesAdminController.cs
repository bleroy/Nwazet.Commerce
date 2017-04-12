using System;
using System.Linq;
using System.Web.Mvc;
using Nwazet.Commerce.Extensions;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Permissions;
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

        public AttributesAdminController(
            IContentManager contentManager,
            ISiteService siteService,
            IShapeFactory shapeFactory,
            IOrchardServices orchardServices) {

            _contentManager = contentManager;
            _siteService = siteService;
            _orchardServices = orchardServices;

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
                result = GenerateAttributeName(displayName),
                version = version
            });
        }

        private string GenerateAttributeName(string displayName) {
            displayName = displayName.ToSafeName();
            var attributes = _contentManager
                .Query<ProductAttributePart>()
                .List();
            while (attributes.Any(at => 
                string.Equals(at.TechnicalName.Trim(), displayName.Trim(), StringComparison.OrdinalIgnoreCase))) {
                displayName = AttributeNameUtilities.VersionName(displayName);
            }
            return displayName;
        }

        
    }
}
