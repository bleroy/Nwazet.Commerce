using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Commerce")]
    public class ProductPartRecord : ContentPartRecord {
        public ProductPartRecord() {
            ShippingCost = null;
        }
        public virtual string Sku { get; set; }
        public virtual double Price { get; set; }
        public virtual double DiscountPrice { get; set; }
        public virtual bool IsDigital { get; set; }
        public virtual double? ShippingCost { get; set; }
        public virtual double Weight { get; set; }
        public virtual string Size { get; set; }
        public virtual int Inventory { get; set; }
        public virtual string OutOfStockMessage { get; set; }
        public virtual bool AllowBackOrder { get; set; }
        public virtual bool OverrideTieredPricing { get; set; }
        public virtual string PriceTiers { get; set; }
        public virtual int MinimumOrderQuantity { get; set; }
        public virtual bool AuthenticationRequired { get; set; }
    }
}
