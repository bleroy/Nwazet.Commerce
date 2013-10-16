using System.ComponentModel.DataAnnotations;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Commerce")]
    public class ProductPart : InfosetContentPart<ProductPartRecord>, IProduct {
        [Required]
        public string Sku {
            get { return Get(r => r.Sku); }
            set { Set(r => r.Sku, value); }
        }

        [Required]
        public double Price {
            get { return Get(r => r.Price); }
            set { Set(r => r.Price, value); }
        }

        public bool IsDigital {
            get { return Get(r => r.IsDigital); }
            set { Set(r => r.IsDigital, value); }
        }

        public double? ShippingCost {
            get { return Get(r => r.ShippingCost); }
            set { Set(r => r.ShippingCost, value); }
        }

        public double Weight {
            get { return Get(r => r.Weight); }
            set { Set(r => r.Weight, value); }
        }

        public string Size {
            get { return Get(r => r.Size); }
            set { Set(r => r.Size, value); }
        }

        public int Inventory {
            get { return Get(r => r.Inventory); }
            set { Set(r => r.Inventory, value); }
        }

        public string OutOfStockMessage {
            get { return Get(r => r.OutOfStockMessage); }
            set { Set(r => r.OutOfStockMessage, value); }
        }

        public bool AllowBackOrder {
            get { return Get(r => r.AllowBackOrder); }
            set { Set(r => r.AllowBackOrder, value); }
        }
    }
}
