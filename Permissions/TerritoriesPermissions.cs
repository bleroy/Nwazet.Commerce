using Nwazet.Commerce.Models;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Environment.Extensions;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nwazet.Commerce.Permissions {
    [OrchardFeature("Territories")]
    public class TerritoriesPermissions : ITerritoriesPermissionProvider {

        #region Base Permissions definitions
        public static readonly Permission ManageTerritories = new Permission {
            Description = "Manage Territories",
            Name = "ManageTerritories"
        };

        public static readonly Permission ManageTerritory = new Permission {
            Description = "Manage Territories of type {0}",
            Name = "ManageTerritories_{0}",
            ImpliedBy = new[] { ManageTerritories }
        };

        public static readonly Permission ManageTerritoryHierarchies = new Permission {
            Description = "Manage hierarchies of territories",
            Name = "ManageTerritoryHierarchies"
        };

        public static readonly Permission ManageTerritoryHierarchy = new Permission {
            Description = "Manage hierarchies of territories of type {0}",
            Name = "ManageTerritoryHierarchies_{0}",
            ImpliedBy = new[] { ManageTerritoryHierarchies }
        };

        public static readonly Permission ManageInternalTerritories = new Permission {
            Description = "Manage the definitions of the allowed Territories.",
            Name = "ManageInternalTerritories"
        };
        #endregion
        
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public TerritoriesPermissions(
            IContentDefinitionManager contentDefinitionManager) {
            
            _contentDefinitionManager = contentDefinitionManager;
        }

        public virtual Feature Feature { get; set; }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes() {
            return Enumerable.Empty<PermissionStereotype>();
        }

        public IEnumerable<Permission> GetPermissions() {
            var permissions = new List<Permission>();
            //Base permissions
            permissions.Add(ManageTerritories);
            permissions.Add(ManageTerritoryHierarchies);
            //Dynamic permissions are defined per type of hierarchy (not per single hierarchy, as
            //is the case for example in menus)
            permissions.AddRange(ListHierarchyTypePermissions());
            permissions.AddRange(ListTerritoryTypePermissions());

            return permissions;
        }

        public IEnumerable<Permission> ListTerritoryTypePermissions() {
            return _contentDefinitionManager.ListTypeDefinitions()
                .Where(ctd => ctd.Parts.Any(pa => pa
                    .PartDefinition.Name.Equals(TerritoryPart.PartName, StringComparison.InvariantCultureIgnoreCase)))
                .Select(ctd => GetTerritoryPermission(ctd));
        }

        public IEnumerable<Permission> ListHierarchyTypePermissions() {
            return _contentDefinitionManager.ListTypeDefinitions()
                .Where(ctd => ctd.Parts.Any(pa => pa
                    .PartDefinition.Name.Equals(TerritoryHierarchyPart.PartName, StringComparison.InvariantCultureIgnoreCase)))
                .Select(ctd => GetHierarchyPermission(ctd));
        }

        /// <summary>
        /// Returns the dynamic permission computed for the type passed as parameter and considered
        /// as a Territory.
        /// </summary>
        /// <param name="typeDefinition">The type for whom the permission will be created.</param>
        /// <returns>The computed dynamic permission.</returns>
        public static Permission GetTerritoryPermission(ContentTypeDefinition typeDefinition) {
            return new Permission {
                Name = string.Format(TerritoriesPermissions.ManageTerritory.Name, typeDefinition.Name),
                Description = string.Format(TerritoriesPermissions.ManageTerritory.Description, typeDefinition.Name),
                ImpliedBy = TerritoriesPermissions.ManageTerritory.ImpliedBy
            };
        }
        
        /// <summary>
        /// Returns the dynamic permission computed for the type passed as parameter and considered
        /// as a Hierarchy.
        /// </summary>
        /// <param name="typeDefinition">The type for whom the permission will be created.</param>
        /// <returns>The computed dynamic permission.</returns>
        public static Permission GetHierarchyPermission(ContentTypeDefinition typeDefinition) {
            return new Permission {
                Name = string.Format(TerritoriesPermissions.ManageTerritoryHierarchy.Name, typeDefinition.Name),
                Description = string.Format(TerritoriesPermissions.ManageTerritoryHierarchy.Description, typeDefinition.Name),
                ImpliedBy = TerritoriesPermissions.ManageTerritoryHierarchy.ImpliedBy
            };
        }

    }
}
