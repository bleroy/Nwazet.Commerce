using System;
using System.Collections.Generic;
using System.Linq;

namespace Nwazet.Commerce.Models {
    [Serializable]
    public sealed class ShoppingCartItem {
        private int _quantity;

        public int ProductId { get; private set; }
        public IDictionary<int, ProductAttributeValueExtended> AttributeIdsToValues { get; set; }

        public int Quantity {
            get { return _quantity; }
            set {
                if (value < 0) throw new IndexOutOfRangeException();
                _quantity = value;
            }
        }

        public ShoppingCartItem() { }

        public ShoppingCartItem(int productId, int quantity = 1, IDictionary<int, ProductAttributeValueExtended> attributeIdsToValues = null) {
            ProductId = productId;
            Quantity = quantity;
            AttributeIdsToValues = attributeIdsToValues;
        }

        public string AttributeDescription {
            get {
                if (AttributeIdsToValues == null || !AttributeIdsToValues.Any()) {
                    return "";
                }
                return "(" + string.Join(", ", AttributeIdsToValues.Values.Select(v => v.Value)) + ")";
            }
        }

        public override string ToString() {
            return "{" + Quantity + " x " + ProductId
                + (string.IsNullOrWhiteSpace(AttributeDescription) ? "" : " " + AttributeDescription)
                + "}";
        }

        public static bool ItemsAreEqual(ShoppingCartItem A, ShoppingCartItem B) {
            if (A.AttributeIdsToValues == null || A.AttributeIdsToValues.Count == 0) {
                return A.ProductId == B.ProductId && (B.AttributeIdsToValues == null || B.AttributeIdsToValues.Count == 0);
            }
            return A.ProductId == B.ProductId
                && B.AttributeIdsToValues != null
                && B.AttributeIdsToValues.Count == A.AttributeIdsToValues.Count
            && B.AttributeIdsToValues.All(A.AttributeIdsToValues.Contains);
        }
    }
}