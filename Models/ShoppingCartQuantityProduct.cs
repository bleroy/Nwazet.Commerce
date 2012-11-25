namespace Nwazet.Commerce.Models {
    public class ShoppingCartQuantityProduct {
        public ShoppingCartQuantityProduct(int quantity, ProductPart product) {
            Quantity = quantity;
            Product = product;
            Price = product.Price;
        }

        public int Quantity { get; private set; }
        public ProductPart Product { get; private set; }
        public double Price { get; set; }
        public string Comment { get; set; }

        public override string ToString() {
            return "{" + Quantity + " " + Product.Sku + " ($" + Price + ")}";
        }
    }
}
