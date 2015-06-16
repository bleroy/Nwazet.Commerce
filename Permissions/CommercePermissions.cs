using System.Collections.Generic;
using Orchard.Environment.Extensions;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace Nwazet.Commerce.Permissions {
    [OrchardFeature("Nwazet.Commerce")]
    public class CommercePermissions : IPermissionProvider {
        public static readonly Permission CommerceAdministrator = new Permission
        {
            Description = "Commerce Administrator",
            Name = "CommerceAdministrator"
        };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions() {
            return new[] {
                CommerceAdministrator
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes() {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {CommerceAdministrator}
                },
                new PermissionStereotype {
                    Name = "Editor"
                },
                new PermissionStereotype {
                    Name = "Moderator"
                },
                new PermissionStereotype {
                    Name = "Author",
                },
                new PermissionStereotype {
                    Name = "Contributor",
                },
            };
        }

    }
}
