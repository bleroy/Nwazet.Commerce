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
            var products = _bundleService.GetProductQuantitiesFor(bundle);
            foreach (var productPartQuantity in products) {
                productPartQuantity.Product.Inventory -= productPartQuantity.Quantity;
            }
            var newInventory = products.ToDictionary(p => p.Product.Sku, p => p.Product.Inventory);
            newInventory.Add(bundle.As<ProductPart>().Sku, products.Min(p => p.Product.Inventory / p.Quantity));
            return new JsonResult {
                Data = newInventory
            };
        }
    }
}
