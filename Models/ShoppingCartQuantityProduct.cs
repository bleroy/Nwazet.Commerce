namespace Nwazet.Commerce.Models {
    public class ShoppingCartQuantityProduct {
        public ShoppingCartQuantityProduct(int quantity, IProduct product) {
            Quantity = quantity;
            Product = product;
        }

        public int Quantity { get; private set; }
        public IProduct Product { get; private set; }
    }
}
