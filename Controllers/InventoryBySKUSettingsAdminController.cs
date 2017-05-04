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

        [HttpGet]
        public ActionResult VerifyInventories() {
            _workContextAccessor.GetContext().CurrentSite.As<InventoryBySKUSiteSettingsPart>().InventoriesAreAllInSynch = false;
            return RedirectToAction("Index", "ECommerceSettingsAdmin");
        }
    }
}
