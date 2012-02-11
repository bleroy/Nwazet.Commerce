using System;

namespace Nwazet.Commerce.Models {
    [Serializable]
    public sealed class ShoppingCartItem {
        private int _quantity;

        public int ProductId { get; private set; }

        public int Quantity {
            get { return _quantity; }
            set {
                if (value < 0) throw new IndexOutOfRangeException();
                _quantity = value;
            }
        }

        public ShoppingCartItem() {}

        public ShoppingCartItem(int productId, int quantity = 1) {
            ProductId = productId;
            Quantity = quantity;
        }
    }
}