using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;

namespace Nwazet.Commerce.Services {
    public interface ITax : IContent {
        string Name { get; }
        int Priority { get; set; }

        /// <summary>
        /// This method computes the total tax on the whole cart.
        /// </summary>
        /// <param name="productQuantities">The products in the cart.</param>
        /// <param name="subtotal">The sum of the prices for all the products in the cart.</param>
        /// <param name="shippingCost">The price of shipping. This may be subject to taxes as well.</param>
        /// <param name="country">The country of destination.</param>
        /// <param name="zipCode">The destination's zip code.</param>
        /// <returns>The sum total of the taxes computed for the cart.</returns>
        decimal ComputeTax(
            IEnumerable<ShoppingCartQuantityProduct> productQuantities,
            decimal subtotal,
            decimal shippingCost,
            string country,
            string zipCode);
    }
}
