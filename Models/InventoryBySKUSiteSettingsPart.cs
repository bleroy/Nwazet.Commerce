using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.InventoryBySKU")]
    public class InventoryBySKUSiteSettingsPart : ContentPart {
        /// <summary>
        /// Tells whether we have already verified the state of inventories. If this flag is true, it means we found that
        /// inventories were synchronized across products. If this flag is false, we either haven't checked, or we found
        /// that not all inventories were synchronized.
        /// </summary>
        public bool InventoriesAreAllInSynch
        {
            get { return this.Retrieve(p => p.InventoriesAreAllInSynch); }
            set { this.Store(p => p.InventoriesAreAllInSynch, value); }
        }
    }
}
