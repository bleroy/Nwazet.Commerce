using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment;
using Orchard.Environment.Extensions.Models;

namespace Nwazet.Commerce.Features {
    public class InventoryBySKUFeature : IFeatureEventHandler {
        private readonly IOrchardServices _orchardServices;
        private readonly IProductInventoryService _productInventoryService;
        public InventoryBySKUFeature(
            IOrchardServices orchardServices,
            IProductInventoryService productInventoryService) {

            _orchardServices = orchardServices;
            _productInventoryService = productInventoryService;
        }

        public void Enabled(Feature feature) {
            if (feature.Descriptor.Id == "Nwazet.InventoryBySKU") {
                //the feature has just been enabled, so maybe the inventories are not in synch
                _orchardServices.WorkContext.CurrentSite.As<InventoryBySKUSiteSettingsPart>().InventoriesAreAllInSynch =
                    !_productInventoryService.GetProductsWithInventoryIssues().Any();
            }
        }

        #region Not used interface methods
        public void Disabled(Feature feature) {
        }

        public void Disabling(Feature feature) {
        }

        public void Enabling(Feature feature) {
        }

        public void Installed(Feature feature) {
        }

        public void Installing(Feature feature) {
        }

        public void Uninstalled(Feature feature) {
        }

        public void Uninstalling(Feature feature) {
        }
        #endregion
    }
}
