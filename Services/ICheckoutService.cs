using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Orchard;

namespace Nwazet.Commerce.Services {
    public interface ICheckoutService : IDependency {
        string Name { get; }

        dynamic BuildCheckoutButtonShape(
            IEnumerable<dynamic> productShapes, 
            IEnumerable<ShoppingCartQuantityProduct> productQuantities,
            IEnumerable<ShippingOption> shippingOptions,
            TaxAmount taxes,
            string country,
            string zipCode,
            IEnumerable<string> custom);

        string GetChargeAdminUrl(string transactionId);
    }
}
