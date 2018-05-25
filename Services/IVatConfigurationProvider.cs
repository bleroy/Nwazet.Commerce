using Nwazet.Commerce.Models;
using Nwazet.Commerce.ViewModels;
using Orchard.ContentManagement;
using System.Collections.Generic;

namespace Nwazet.Commerce.Services {
    public interface IVatConfigurationProvider : ITaxProvider {
        /// <summary>
        /// Get all the published VAT Category configurations
        /// </summary>
        /// <returns></returns>
        IEnumerable<VatConfigurationPart> GetVatConfigurations();

        /// <summary>
        /// Get all the VAT Category configurations in the specified version.
        /// </summary>
        /// <param name="versionOptions">The desired version.</param>
        /// <returns></returns>
        IEnumerable<VatConfigurationPart> GetVatConfigurations(VersionOptions versionOptions);

        /// <summary>
        /// Updates the VAT category configurations for the given hierarchy.
        /// </summary>
        /// <param name="part">The HierarchyVatConfigurationPart whose information we are updating.</param>
        /// <param name="model">The HierarchyVatConfigurationPartViewModel object that contains the updated information.</param>
        void UpdateConfiguration(
            HierarchyVatConfigurationPart part, HierarchyVatConfigurationPartViewModel model);

        /// <summary>
        /// Updates the VAT category configurations for the given territory.
        /// </summary>
        /// <param name="part">The TerritoryVatConfigurationPart whose information we are updating.</param>
        /// <param name="model">The TerritoryVatConfigurationPartViewModel object that contains the updated information.</param>
        void UpdateConfiguration(
            TerritoryVatConfigurationPart part, TerritoryVatConfigurationPartViewModel model);

        /// <summary>
        /// Clear the records for the VAT configurations for the product category represented by the given part.
        /// </summary>
        /// <param name="part">The part whose configurations are to be deleted.</param>
        void ClearIntersectionRecords(VatConfigurationPart part);
        
        /// <summary>
        /// Clear the records for the VAT configurations for the hierarchy represented by the given part.
        /// </summary>
        /// <param name="part">The part whose configurations are to be deleted.</param>
        void ClearIntersectionRecords(HierarchyVatConfigurationPart part);

        /// <summary>
        /// Clear the records for the VAT configurations for the territory represented by the given part.
        /// </summary>
        /// <param name="part">The part whose configurations are to be deleted.</param>
        void ClearIntersectionRecords(TerritoryVatConfigurationPart part);
    }
}
