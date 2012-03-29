using Nwazet.Commerce.Models;
using Orchard.ContentManagement;

namespace Nwazet.Commerce.Tests {
    public class ProductStub : ProductPart {
        public ProductStub() {
            _record.Value = new ProductPartRecord();
            ShippingCost = -1;
        }
    }
}
