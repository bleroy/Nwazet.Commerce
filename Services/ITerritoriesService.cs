using Nwazet.Commerce.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Models;
using System.Collections.Generic;

namespace Nwazet.Commerce.Services {
    public interface ITerritoriesService : IDependency {
        /// <summary>
        /// Returns the type definitions for the types of territory hierarchies that
        /// the user is allowed to manage.
        /// </summary>
        /// <returns>An Enumerable of the ContentTypeDefinitions for all types 
        /// that the current user is allowed to manage.</returns>
        IEnumerable<ContentTypeDefinition> GetHierarchyTypes();
        
        /// <summary>
        /// Returns the type definitions for the types of territories that
        /// the user is allowed to manage.
        /// </summary>
        /// <returns>An Enumerable of the ContentTypeDefinitions for all types 
        /// that the current user is allowed to manage</returns>
        IEnumerable<ContentTypeDefinition> GetTerritoryTypes();
        
        /// <summary>
        /// Provides an IContentQuery for the Latest versions of TerritoryHierarchyParts
        /// </summary>
        /// <returns></returns>
        IContentQuery<TerritoryHierarchyPart, TerritoryHierarchyPartRecord> GetHierarchiesQuery();

        /// <summary>
        /// Provides an IContentQuery for the latest versions of TerritoryHierarchyParts from the specific ContentTypes.
        /// </summary>
        /// <param name="contentTypes">The names of the ContentTypes.</param>
        /// <returns></returns>
        IContentQuery<TerritoryHierarchyPart> GetHierarchiesQuery(params string[] contentTypes);

        /// <summary>
        /// Provides an IContentQuery for the specific versions of TerritoryHierarchyParts
        /// </summary>
        /// <param name="versionOptions">The version for the items. Defaults at Latest.</param>
        /// <returns></returns>
        IContentQuery<TerritoryHierarchyPart, TerritoryHierarchyPartRecord> GetHierarchiesQuery(VersionOptions versionOptions);

        /// <summary>
        /// Provides an IContentQuery for the specific versions of TerritoryHierarchyParts from the specific ContentTypes.
        /// </summary>
        /// <param name="versionOptions">The version for the items.</param>
        /// <param name="contentTypes">Teh names of the ContentTypes.</param>
        /// <returns></returns>
        IContentQuery<TerritoryHierarchyPart> GetHierarchiesQuery(VersionOptions versionOptions, params string[] contentTypes);

        /// <summary>
        /// Provides an IContentQuery for the TerritoryParts in a given hierarchy. If the hierarchy is Published, this
        /// returns Published territories. Otherwise it returns the Latest version.
        /// </summary>
        /// <param name="hierarchyPart">The hierarchy that the territories belong to.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Throws an ArgumentNullException if the TerritoryHierarchyPart
        /// argument is null.</exception>
        /// <exception cref="ArgumentException">Throws an ArgumentException if the TerritoryHierarchyPart
        /// argument has a null underlying record.</exception>
        IContentQuery<TerritoryPart, TerritoryPartRecord> GetTerritoriesQuery(TerritoryHierarchyPart hierarchyPart);

        /// <summary>
        /// Provides an IContentQuery for the TerritoryParts in a given hierarchy.
        /// </summary>
        /// <param name="hierarchyPart">The hierarchy that the territories belong to.</param>
        /// <param name="versionOptions">The version for the items. Defaults to the version of the item of the hierarchyPart,
        /// falling back to Latest.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Throws an ArgumentNullException if the TerritoryHierarchyPart
        /// argument is null.</exception>
        /// <exception cref="ArgumentException">Throws an ArgumentException if the TerritoryHierarchyPart
        /// argument has a null underlying record.</exception>
        IContentQuery<TerritoryPart, TerritoryPartRecord> GetTerritoriesQuery(TerritoryHierarchyPart hierarchyPart, VersionOptions versionOptions);

        /// <summary>
        /// Provides an IContentQuery for the TerritoryParts in a given hierarchy, whose parent is the one specified. 
        /// The version of the territories is the version of the hierarchy, or Latest.
        /// </summary>
        /// <param name="hierarchyPart">The hierarchy that the territories belong to</param>
        /// <param name="territoryPart">The parent territory whose children will be returned by the query. If this is null,
        /// the query will return the first level of the hierarchy.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Throws an ArgumentNullException if the TerritoryHierarchyPart
        /// argument is null.</exception>
        /// <exception cref="ArgumentException">Throws an ArgumentException if the TerritoryHierarchyPart
        /// argument has a null underlying record.</exception>
        IContentQuery<TerritoryPart, TerritoryPartRecord> GetTerritoriesQuery(TerritoryHierarchyPart hierarchyPart, TerritoryPart territoryPart);

        /// <summary>
        /// Provides an IContentQuery for the TerritoryParts in a given hierarchy, whose parent is the one specified. 
        /// The version of the territories is the version of the hierarchy, or Latest.
        /// </summary>
        /// <param name="hierarchyPart">The hierarchy that the territories belong to</param>
        /// <param name="territoryPart">The parent territory whose children will be returned by the query. If this is null,
        /// the query will return the first level of the hierarchy.</param>
        /// <param name="versionOptions">The version for the items. Defaults to the version of the item of the hierarchyPart,
        /// falling back to Latest.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Throws an ArgumentNullException if the TerritoryHierarchyPart
        /// argument is null.</exception>
        /// <exception cref="ArgumentException">Throws an ArgumentException if the TerritoryHierarchyPart
        /// argument has a null underlying record.</exception>
        IContentQuery<TerritoryPart, TerritoryPartRecord> GetTerritoriesQuery(TerritoryHierarchyPart hierarchyPart, TerritoryPart territoryPart, VersionOptions versionOptions);

        /// <summary>
        /// Fetch a list of the TerritoryInternalRecords that do not yet have a corresponding ContentItem
        /// in the hierarchy that we are processing.
        /// </summary>
        /// <param name="hierarchy">The hierarchy we are working on.</param>
        /// <returns>An IEnumerable of the TerritoryInternalRecord that have not yet been used in the current hierarchy.</returns>
        /// <exception cref="ArgumentNullException">Throws an ArgumentNullException if the TerritoryHierarchyPart
        /// argument is null.</exception>
        /// <exception cref="ArgumentException">Throws an ArgumentException if the TerritoryHierarchyPart
        /// argument has a null underlying record.</exception>
        IEnumerable<TerritoryInternalRecord> GetAvailableTerritoryInternals(TerritoryHierarchyPart hierarchyPart);
    }
}
