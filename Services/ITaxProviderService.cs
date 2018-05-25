using Nwazet.Commerce.Models;
using Orchard;
using System.Collections.Generic;

namespace Nwazet.Commerce.Services {
    public interface ITaxProviderService : IDependency {
        /// <summary>
        /// Creates a context object that contains the given information.
        /// </summary>
        /// <param name="productQuantities"></param>
        /// <param name="subtotal"></param>
        /// <param name="shippingCost"></param>
        /// <param name="country"></param>
        /// <param name="zipCode"></param>
        /// <returns></returns>
        TaxContext CreateContext(
            IEnumerable<ShoppingCartQuantityProduct> productQuantities,
            decimal subtotal,
            decimal shippingCost,
            string country, string zipCode);

        /// <summary>
        /// Computes the total due taxes for the given parameters.
        /// </summary>
        /// <param name="tax"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        decimal TotalTaxes(ITax tax, TaxContext context);

        /// <summary>
        /// Computes itemized taxes for each item in the context.
        /// </summary>
        /// <param name="tax"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        IEnumerable<decimal> ItemizedTaxes(ITax tax, TaxContext context);

        /// <summary>
        /// Computes shipping taxes for the context.
        /// </summary>
        /// <param name="tax"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        decimal ShippingTaxes(ITax tax, TaxContext context);
    }
}
