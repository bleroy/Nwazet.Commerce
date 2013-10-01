using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;

namespace Nwazet.Commerce.Services {
    public interface ITax : IContent {
        string Name { get; }
        int Priority { get; set; }

        double ComputeTax(
            IEnumerable<ShoppingCartQuantityProduct> productQuantities,
            double subtotal,
            double shippingCost,
            string country,
            string zipCode);
    }
}
