using Nwazet.Commerce.Models;
using System.Collections.Generic;

namespace Nwazet.Commerce.Services {
    /// <summary>
    /// This class is used to wrap the information required for computation of the correct
    /// tax amounts.
    /// </summary>
    public class TaxContext {
        #region Properties required for compatibility with old tax feature
        public IEnumerable<ShoppingCartQuantityProduct> ShoppingCartQuantityProducts { get; set; }
        public decimal CartSubTotal { get; set; }
        public decimal ShippingPrice { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        #endregion
        
        /// <summary>
        /// We are going to need a destination for taxes and shipping.
        /// </summary>
        public TerritoryInternalRecord DestinationTerritory { get; set; }
    }
}
