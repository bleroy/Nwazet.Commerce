using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Contents.ViewModels;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.UI.Admin;
using Orchard.Security;
using Orchard.Settings;
using Orchard.UI.Navigation;

namespace Nwazet.Commerce.Controllers {
    [OrchardFeature("Nwazet.Commerce")]
    [ValidateInput(false), Admin]
    public class ProductAdminController : Controller {
        private readonly IContentManager _contentManager;
        private readonly ISiteService _siteService;
        private readonly IWorkContextAccessor _wca;
        private readonly IOrchardServices _orchardServices;

        public ProductAdminController(
            IOrchardServices services,
            IContentManager contentManager,
            ISiteService siteService,
            IWorkContextAccessor wca,
            IShapeFactory shapeFactory,
            IOrchardServices orchardServices) {
            Services = services;
            _contentManager = contentManager;
            _siteService = siteService;
            _wca = wca;
            T = NullLocalizer.Instance;
            Shape = shapeFactory;
            _orchardServices = orchardServices;
        }

        dynamic Shape { get; set; }
        public Localizer T { get; set; }
        public IOrchardServices Services { get; set; }

        public ActionResult List(ListContentsViewModel model, PagerParameters pagerParameters) {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner, null, T("Not authorized to manage products"))) 
                return new HttpUnauthorizedResult();
            
            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);
            var query = _contentManager.Query<ProductPart, ProductPartRecord>(VersionOptions.Latest);

            switch (model.Options.OrderBy) {
                case ContentsOrder.Modified:
                    query.OrderByDescending<CommonPartRecord>(cr => cr.ModifiedUtc);
                    break;
                case ContentsOrder.Published:
                    query.OrderByDescending<CommonPartRecord>(cr => cr.PublishedUtc);
                    break;
                case ContentsOrder.Created:
                    query.OrderByDescending<CommonPartRecord>(cr => cr.CreatedUtc);
                    break;
            }

            var pagerShape = Shape.Pager(pager).TotalItemCount(query.Count());
            var pageOfContentItems = query.Slice(pager.GetStartIndex(), pager.PageSize).ToList();

            var list = Shape.List();
            list.AddRange(pageOfContentItems.Select(ci => _contentManager.BuildDisplay(ci, "SummaryAdmin")));

            dynamic viewModel = Shape.ViewModel()
                .ContentItems(list)
                .Pager(pagerShape)
                .Options(model.Options);

            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            return View((object)viewModel);
        }

        [HttpPost]
        public ActionResult RemoveOne(int id) {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner, null, T("Not authorized to manage products")))
                return new HttpUnauthorizedResult();            

            var product = _contentManager.Get<ProductPart>(id);
            product.Inventory--;
            Dictionary<string, int> newInventory;
            IBundleService bundleService;
            if (_wca.GetContext().TryResolve(out bundleService)) {
                var affectedBundles = _contentManager.Query<BundlePart, BundlePartRecord>()
                    .Where(b => b.Products.Any(p => p.ContentItemRecord.Id == product.Id))
                    .WithQueryHints(new QueryHints().ExpandParts<ProductPart>())
                    .List();
                newInventory = affectedBundles.ToDictionary(
                    b => b.As<ProductPart>().Sku,
                    b => bundleService.GetProductQuantitiesFor(b).Min(p => p.Product.Inventory / p.Quantity));
            } else {
                newInventory = new Dictionary<string, int>(1);
            }
            newInventory.Add(product.Sku, product.Inventory);
            return new JsonResult {
                Data = newInventory
            };
        }
    }
}