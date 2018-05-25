using Nwazet.Commerce.Models;
using Orchard;

namespace Nwazet.Commerce.Services {
    public interface IVatConfigurationService : IDependency {

        /// <summary>
        /// Gets the Id of the product category that is set as the default one to be used. The default
        /// category is the product category that will be implicitly assigned to products whenever they
        /// have no category assigned to them explicitly.
        /// </summary>
        /// <returns>The Id of the default product category. The return value will be 0 if no default
        /// category is set.</returns>
        /// <remarks>This method should only consider published categories.</remarks>
        int GetDefaultCategoryId();

        /// <summary>
        /// Get the object representing the default product category to be used when evaluating the 
        /// VAT for products where a specific category has not been set.
        /// </summary>
        /// <returns>A object describing the default VAT to be applied. This method will return null
        /// if it is impossible to determine a default category.</returns>
        /// <remarks>This method should only consider published categories.</remarks>
        VatConfigurationPart GetDefaultCategory();

        /// <summary>
        /// Set the category represented by the object as the default one to be used when no other
        /// category has been explicitly set.
        /// </summary>
        /// <param name="part">The object describing the default VAT to be used.</param>
        void SetDefaultCategory(VatConfigurationPart part);

        /// <summary>
        /// Given a product's configuration, find the VAT rate to apply.
        /// </summary>
        /// <param name="part">The ProductPart for the product</param>
        /// <returns>The rate computed for the product and the default destination.</returns>
        decimal GetRate(ProductPart part);

        /// <summary>
        /// Given a product's configuration and a destination terriotry, find the VAT rate to apply.
        /// </summary>
        /// <param name="part">The ProductPart for the product</param>
        /// <param name="destination">An object describing the destination.</param>
        /// <returns>The rate computed for the product and the destination.</returns>
        /// <remarks>The case destination == null should be handled by computing the rate for 
        /// the default destination in the settings for the site.</remarks>
        decimal GetRate(ProductPart part, TerritoryInternalRecord destination);

        /// <summary>
        /// Get the default destination configured for the purpose of VAT computations.
        /// </summary>
        /// <returns>Returns the territory selected as default destination, or null if none is selected.</returns>
        TerritoryInternalRecord GetDefaultDestination();
    }
}
