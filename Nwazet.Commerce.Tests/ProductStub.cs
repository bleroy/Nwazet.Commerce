using Nwazet.Commerce.Models;

namespace Nwazet.Commerce.Tests {
    public class ProductStub : IProduct {
        public ProductStub() {
            ShippingCost = -1;
        }

        public int Id { get; set; }
        public string Sku { get; set; }
        public double Price { get; set; }
        public bool IsDigital { get; set; }
        public double? ShippingCost { get; set; }
        public double Weight { get; set; }
    }
}
