using Nwazet.Commerce.Models;
using Orchard;
using System.Collections.Generic;

namespace Nwazet.Commerce.Services {
    /// <summary>
    /// The methods in this interface are used to compute the price displayed on the frontend.
    /// </summary>
    public interface IProductPriceService : IDependency {
        /// <summary>
        /// Returns the price for the product before any discount or taxes are applied
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        decimal GetOriginalPrice(ProductPart part);

        /// <summary>
        /// Returns the price for the product
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        decimal GetPrice(ProductPart part);

        /// <summary>
        /// Returns the discounted price for the product
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        decimal GetDiscountPrice(ProductPart part);

        /// <summary>
        /// Returns the shipping cost for the product
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        decimal? GetShippingCost(ProductPart part);

        /// <summary>
        /// Returns the Price tiers for the product
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        IEnumerable<PriceTier> GetPriceTiers(ProductPart part);

        /// <summary>
        /// Computes a price based on the ProductPart's properties and the given base price
        /// </summary>
        /// <param name="part">The ProductPart whose properties will be used to compute the changes to a price.</param>
        /// <param name="basePrice">The base price.</param>
        /// <returns>A modified price based on the properties of the ProductPart</returns>
        /// <remarks>This method is used to compute price changes when the Price property being affected is 
        /// not the part's, but rather something derived from it. For example, it may be the price resulting from
        /// applying changes due to attributes, discounts and the likes.</remarks>
        decimal GetPrice(ProductPart part, decimal basePrice);

        /// <summary>
        /// Get the price for the product for the given destination.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="country"></param>
        /// <param name="zipCode"></param>
        /// <returns></returns>
        decimal GetPrice(ProductPart part, string country, string zipCode);

        /// <summary>
        /// Computes a price based on the ProductPart's properties and the given base price for 
        /// the given destination.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="basePrice"></param>
        /// <param name="country"></param>
        /// <param name="zipCode"></param>
        /// <returns></returns>
        decimal GetPrice(ProductPart part, decimal basePrice, string country, string zipCode);

        /// <summary>
        /// Returns the discounted price for the product for the given destination.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="country"></param>
        /// <param name="zipCode"></param>
        /// <returns></returns>
        decimal GetDiscountPrice(ProductPart part, string country, string zipCode);
    }
}
