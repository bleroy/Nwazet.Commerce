using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;

namespace Nwazet.Commerce.Services {
    public interface ITax : IContent {
        string Name { get; }
        int Priority { get; set; }

        decimal ComputeTax(
            IEnumerable<ShoppingCartQuantityProduct> productQuantities,
            decimal subtotal,
            decimal shippingCost,
            string country,
            string zipCode);
    }
}
