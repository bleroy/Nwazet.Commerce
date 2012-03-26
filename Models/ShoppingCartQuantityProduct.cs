namespace Nwazet.Commerce.Models {
    public class ShoppingCartQuantityProduct {
        public ShoppingCartQuantityProduct(int quantity, ProductPart product) {
            Quantity = quantity;
            Product = product;
        }

        public int Quantity { get; private set; }
        public ProductPart Product { get; private set; }
    }
}
