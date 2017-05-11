using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;

namespace Nwazet.Commerce.ViewModels {
    public class InventoryBySKUProductEditorViewModel {
        public ProductPart Product { get; set; }
        public IEnumerable<ProductPart> SameInventoryItems { get; set; }

        public InventoryBySKUProductEditorViewModel() {
            SameInventoryItems = new List<ProductPart>();
        }
    }
}
