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
        private readonly IProductInventoryService _productInventoryService;

        public BundleAdminController(IBundleService bundleService, 
            IContentManager contentManager,
            IProductInventoryService productInventoryService) {

            _bundleService = bundleService;
            _contentManager = contentManager;
            _productInventoryService = productInventoryService;
        }

        [HttpPost]
        public ActionResult RemoveOne(int id) {
            var bundle = _contentManager.Get<BundlePart>(id);
            var products = _bundleService.GetProductQuantitiesFor(bundle).ToList();
            foreach (var productPartQuantity in products) {
                //These calls will also update the inventory for the bundle
                _productInventoryService.UpdateInventory(productPartQuantity.Product, -productPartQuantity.Quantity);
            }
            var newInventory = products.ToDictionary(p => p.Product.Sku, p => _productInventoryService.GetInventory(p.Product));
            newInventory.Add(bundle.As<ProductPart>().Sku, products.Min(p => _productInventoryService.GetInventory(p.Product) / p.Quantity));
            return new JsonResult {
                Data = newInventory
            };
        }
    }
}
