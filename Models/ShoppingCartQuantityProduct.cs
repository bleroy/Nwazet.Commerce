using Orchard.ContentManagement;
using System.Collections.Generic;
using System.Linq;

namespace Nwazet.Commerce.Models {
    public class ShoppingCartQuantityProduct {
        public ShoppingCartQuantityProduct(int quantity, ProductPart product, IDictionary<int, string> attributeIdsToValues = null) {
            Quantity = quantity;
            Product = product;
            Price = product.Price;
            AttributeIdsToValues = attributeIdsToValues;
        }

        public int Quantity { get; private set; }
        public ProductPart Product { get; private set; }
        public double Price { get; set; }
        // Captures attribute price adjustments that apply to the line item, not each item
        public double LinePriceAdjustment { get; set; }
        public IContent Promotion { get; set; }
        public string Comment { get; set; }
        public IDictionary<int, string> AttributeIdsToValues { get; set; }

        public string AttributeDescription {
            get {
                if (AttributeIdsToValues == null || !AttributeIdsToValues.Any()) {
                    return "";
                }
                return "(" + string.Join(", ", AttributeIdsToValues.Values) + ")";
            }
        }

        public override string ToString() {
            return Quantity + " " + Product.Sku 
                + (string.IsNullOrWhiteSpace(AttributeDescription) ? "" : " " + AttributeDescription) 
                + " at $" + Price;
        }
    }
}
