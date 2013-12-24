using Nwazet.Commerce.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nwazet.Commerce.ViewModels {
    public class ProductEditorViewModel {
        public ProductPart Part { get; set; }
        public string Sku { get; set; }
        public double Price { get; set; }
        public bool IsDigital { get; set; }
        public double? ShippingCost { get; set; }
        public double Weight { get; set; }
        public string Size { get; set; }
        public int Inventory { get; set; }
        public string OutOfStockMessage { get; set; }
        public bool AllowBackOrder { get; set; }
        public bool AllowProductOverrides { get; set; }
        public bool OverrideTieredPricing { get; set; }
        public ICollection<PriceTierViewModel> PriceTiers { get; set; }
    }

    public class PriceTierViewModel {
        public int Quantity { get; set; }
        [RegularExpression(@"^\$?\d+(,\d{3})*(\.\d*)?%?$", ErrorMessage = "Tier price is not valid")]
        public string Price { get; set; }
    }
}
