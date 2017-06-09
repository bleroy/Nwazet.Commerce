using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Environment.Extensions;
using Orchard.UI.Admin;

namespace Nwazet.Commerce.Controllers {
    [OrchardFeature("Nwazet.Bundles")]
    [Admin]
    public class BundleAdminController : Controller {
        private readonly IBundleService _bundleService;
        private readonly IBundleAutocompleteService _bundleAutocompleteService;
        private readonly IContentManager _contentManager;
        private readonly IProductInventoryService _productInventoryService;

        public BundleAdminController(
            IBundleService bundleService,
            IBundleAutocompleteService bundleAutocompleteService,
            IContentManager contentManager,
            IProductInventoryService productInventoryService) {

            _bundleService = bundleService;
            _bundleAutocompleteService = bundleAutocompleteService;
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

        [HttpPost]
        public ActionResult SearchProduct(string searchText,List<int> excludedProductIds) {
           var model =  _bundleAutocompleteService.GetProducts(searchText, excludedProductIds);
           return Json(model);
         }
    }
}
