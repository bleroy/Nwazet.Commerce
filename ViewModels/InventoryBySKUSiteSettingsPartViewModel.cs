using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;

namespace Nwazet.Commerce.ViewModels {
    public class InventoryBySKUSiteSettingsPartViewModel {
        public IEnumerable<ProductPart> BadProducts { get; set; }
        public bool InventoriesNeedSynch { get; set; }

        public InventoryBySKUSiteSettingsPartViewModel() {
            BadProducts = new List<ProductPart>();
        }
    }
}
