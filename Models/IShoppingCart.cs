using System;
using System.Collections.Generic;
using Orchard;

namespace Nwazet.Commerce.Models {
    public interface IShoppingCart : IDependency {
        IEnumerable<ShoppingCartItem> Items { get; }
        void Add(int productId, int quantity = 1);
        void AddRange(IEnumerable<ShoppingCartItem> items);
        void Remove(int productId);
        ProductPart GetProduct(int productId);
        IEnumerable<ShoppingCartQuantityProduct> GetProducts();
        void UpdateItems();
        double Subtotal();
        double Taxes();
        double Total();
        double ItemCount();
        void Clear();
    }
}