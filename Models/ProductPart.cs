using System.ComponentModel.DataAnnotations;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Commerce")]
    public class ProductPart : InfosetContentPart<ProductPartRecord>, IProduct {
        [Required]
        public string Sku {
            get { return Retrieve(r => r.Sku); }
            set { Store(r => r.Sku, value); }
        }

        [Required]
        public double Price {
            get { return Retrieve(r => r.Price); }
            set { Store(r => r.Price, value); }
        }

        public bool IsDigital {
            get { return Retrieve(r => r.IsDigital); }
            set { Store(r => r.IsDigital, value); }
        }

        public double? ShippingCost {
            get { return Retrieve(r => r.ShippingCost); }
            set { Store(r => r.ShippingCost, value); }
        }

        public double Weight {
            get { return Retrieve(r => r.Weight); }
            set { Store(r => r.Weight, value); }
        }

        public string Size {
            get { return Retrieve(r => r.Size); }
            set { Store(r => r.Size, value); }
        }

        public int Inventory {
            get { return Retrieve(r => r.Inventory); }
            set { Store(r => r.Inventory, value); }
        }

        public string OutOfStockMessage {
            get { return Retrieve(r => r.OutOfStockMessage); }
            set { Store(r => r.OutOfStockMessage, value); }
        }

        public bool AllowBackOrder {
            get { return Retrieve(r => r.AllowBackOrder); }
            set { Store(r => r.AllowBackOrder, value); }
        }
    }
}
