using System.Collections.Generic;
using Orchard.Environment.Extensions;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace Nwazet.Commerce.Permissions {
    [OrchardFeature("Nwazet.Orders")]
    public class OrderPermissions : IPermissionProvider {
        public static readonly Permission ManageOrders = new Permission {
            Description = "Manage orders",
            Name = "ManageOrders"
        };

        public static readonly Permission ViewAllOrders = new Permission {
            Description = "View all orders",
            Name = "ViewAllOrders",
            ImpliedBy = new[] { ManageOrders }
        };

        public static readonly Permission ViewOwnOrders = new Permission {
            Description = "View own orders",
            Name = "ViewOwnOrders",
            ImpliedBy = new[] { ManageOrders, ViewAllOrders }
        };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions() {
            return new[] {
                ManageOrders,
                ViewOwnOrders,
                ViewAllOrders
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes() {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] { ManageOrders, ViewOwnOrders, ViewAllOrders }
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
