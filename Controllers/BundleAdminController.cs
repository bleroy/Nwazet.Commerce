using System.Linq;
using System.Web.Mvc;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.UI.Admin;

namespace Nwazet.Commerce.Controllers {
    [OrchardFeature("Nwazet.Bundles")]
    [Admin]
    public class BundleAdminController : Controller {
        private readonly IBundleService _bundleService;
        private readonly IContentManager _contentManager;

        public BundleAdminController(IBundleService bundleService, IContentManager contentManager) {
            _bundleService = bundleService;
            _contentManager = contentManager;
        }

        [HttpPost]
        public ActionResult RemoveOne(int id) {
            var bundle = _contentManager.Get<BundlePart>(id);
            var products = _bundleService.GetProductsFor(bundle);
            foreach (var productPart in products) {
                productPart.Inventory--;
            }
            var newInventory = products.ToDictionary(p => p.Sku, p => p.Inventory);
            newInventory.Add(bundle.As<ProductPart>().Sku, products.Min(p => p.Inventory));
            return new JsonResult {
                Data = newInventory
            };
        }
    }
}
