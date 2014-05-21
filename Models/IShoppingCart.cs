using System.Collections.Generic;
using Orchard;

namespace Nwazet.Commerce.Models {
    public interface IShoppingCart : IDependency {
        IEnumerable<ShoppingCartItem> Items { get; }
        string Country { get; set; }
        string ZipCode { get; set; }
        ShippingOption ShippingOption { get; set; }
        void Add(int productId, int quantity = 1, IDictionary<int, string> attributeIdsToValues = null);
        void AddRange(IEnumerable<ShoppingCartItem> items);
        void Remove(int productId, IDictionary<int, string> attributeIdsToValues = null);
        IEnumerable<ShoppingCartQuantityProduct> GetProducts();
        ShoppingCartItem FindCartItem(int productId, IDictionary<int, string> attributeIdsToValues);
        void UpdateItems();
        double Subtotal();
        TaxAmount Taxes(double subTotal = 0);
        double Total(double subTotal = 0, TaxAmount taxes = null);
        double ItemCount();
        void Clear();
    }
}