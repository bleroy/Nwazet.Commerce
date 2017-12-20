using Orchard.Security.Permissions;
using System.Collections.Generic;

namespace Nwazet.Commerce.Permissions {
    public interface ITerritoriesPermissionProvider : IPermissionProvider {
        /// <summary>
        /// Returns the "Manage" permissions for all types of territory.
        /// </summary>
        /// <returns>The list of the dynamic permissions for the territories.</returns>
        /// <remarks>This method does not filter out the permissions that the current user does not have. 
        /// Rather, it returns all possible permissions for the types of territories.</remarks>
        IEnumerable<Permission> ListTerritoryTypePermissions();

        /// <summary>
        /// Returns the "Manage" permissions for all types of territory hierarchy.
        /// </summary>
        /// <returns>The list of the dynamic permissions for the territory hierarchies.</returns>
        /// <remarks>This method does not filter out the permissions that the current user does not have. 
        /// Rather, it returns all possible permissions for the types of territory hierarchies.</remarks>
        IEnumerable<Permission> ListHierarchyTypePermissions();
        
    }
}
