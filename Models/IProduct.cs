namespace Nwazet.Commerce.Models {
    public interface IProduct {
        int Id { get; }
        string Sku { get; set; }
        decimal Price { get; set; }
        bool IsDigital { get; set; }
        decimal? ShippingCost { get; set; }
        double Weight { get; set; }
    }
}
