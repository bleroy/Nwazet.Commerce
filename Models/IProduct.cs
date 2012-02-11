using Orchard.ContentManagement;

namespace Nwazet.Commerce.Models {
    public interface IProduct {
        int Id { get; }
        string Sku { get; set; }
        double Price { get; set; }
        bool IsDigital { get; set; }
        double? ShippingCost { get; set; }
        double Weight { get; set; }
    }
}
