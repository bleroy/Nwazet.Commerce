using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.UI.Admin;

namespace Nwazet.Commerce.Controllers {
    [Admin]
    [OrchardFeature("Nwazet.InventoryBySKU")]
    public class InventoryBySKUSettingsAdminController : Controller {
        private readonly IProductInventoryService _productInventoryService;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IContentManager _contentManager;
        public InventoryBySKUSettingsAdminController(
            IProductInventoryService productInventoryService,
            IWorkContextAccessor workContextAccessor,
            IContentManager contentManager) {

            _productInventoryService = productInventoryService;
            _workContextAccessor = workContextAccessor;
            _contentManager = contentManager;
        }

        [HttpPost]
        public ActionResult VerifyInventories() {
            List<ProductPart> badProducts = _productInventoryService.GetProductsWithInventoryIssues().ToList();
            _workContextAccessor.GetContext().CurrentSite.As<InventoryBySKUSiteSettingsPart>().InventoriesAreAllInSynch =
                    !badProducts.Any();
            return Json(new {
                Result = badProducts.Any().ToString().ToUpperInvariant(),
                BadProductsLinks = badProducts.Select(bp => _contentManager.GetItemMetadata(bp.ContentItem).EditorRouteValues).ToArray(),
                badProductsText = badProducts.Select(bp => _contentManager.GetItemMetadata(bp.ContentItem).DisplayText).ToArray()
            });
        }
    }
}
