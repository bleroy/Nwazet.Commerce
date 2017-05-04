using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Nwazet.InventoryBySKU")]
    public class InventoryBySKUSiteSettingsPartDriver : ContentPartDriver<InventoryBySKUSiteSettingsPart> {

        private readonly IProductInventoryService _productInventoryService;
        private readonly IWorkContextAccessor _workContextAccessor;

        public InventoryBySKUSiteSettingsPartDriver(
            IProductInventoryService productInventoryService,
            IWorkContextAccessor workContextAccessor) {

            _productInventoryService = productInventoryService;
            _workContextAccessor = workContextAccessor;
        }

        protected override string Prefix
        {
            get
            {
                return "InventoryBySKUSiteSettings";
            }
        }

        protected override DriverResult Editor(InventoryBySKUSiteSettingsPart part, dynamic shapeHelper) {
            bool synchRequired = !_workContextAccessor.GetContext()
                    .CurrentSite.As<InventoryBySKUSiteSettingsPart>().InventoriesAreAllInSynch;

            List<ProductPart> badProducts = new List<ProductPart>();
            if (synchRequired) {
                badProducts = _productInventoryService.GetProductsWithInventoryIssues().ToList();
                synchRequired = badProducts.Any();
            }
            if (!synchRequired) {
                _workContextAccessor.GetContext()
                    .CurrentSite.As<InventoryBySKUSiteSettingsPart>().InventoriesAreAllInSynch = true;
            }

            var model = new InventoryBySKUSiteSettingsPartViewModel() {
                InventoriesNeedSynch = synchRequired,
                BadProducts = badProducts
            };

            return ContentShape("SiteSettings_InventoryBySKU",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "SiteSettings/InventoryBySKU",
                    Model: model,
                    Prefix: Prefix
                    )).OnGroup("ECommerceSiteSettings");
        }

        protected override DriverResult Editor(InventoryBySKUSiteSettingsPart part, IUpdateModel updater, dynamic shapeHelper) {
            return Editor(part, shapeHelper);
        }
    }
}
