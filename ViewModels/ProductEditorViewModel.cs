using Nwazet.Commerce.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nwazet.Commerce.ViewModels {
    public class ProductEditorViewModel {
        public ProductPart Product { get; set; }
        public bool AllowProductOverrides { get; set; }
        public ICollection<PriceTierViewModel> PriceTiers { get; set; }
        public decimal? DiscountPrice { get; set; }
    }

    public class PriceTierViewModel {
        public int Quantity { get; set; }
        [RegularExpression(@"^\$?\d+(,\d{3})*(\.\d*)?%?$", ErrorMessage = "Tier price is not valid")]
        public string Price { get; set; }
    }
}
