using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment;
using Orchard.Environment.Extensions.Models;

namespace Nwazet.Commerce.Features {
    public class InventoryBySKUFeature : IFeatureEventHandler {
        private readonly IOrchardServices _orchardServices;
        public InventoryBySKUFeature(
            IOrchardServices orchardServices) {

            _orchardServices = orchardServices;
        }

        public void Enabled(Feature feature) {
            if (feature.Descriptor.Id == "Nwazet.InventoryBySKU") {
                //the feature has just been enabled, so maybe the inventories are not in synch
                _orchardServices.WorkContext.CurrentSite.As<InventoryBySKUSiteSettingsPart>().InventoriesAreAllInSynch = false;
                //check whether inventories are already synchronized
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
