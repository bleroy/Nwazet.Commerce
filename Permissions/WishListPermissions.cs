using Orchard.Environment.Extensions;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;
using System.Collections.Generic;

namespace Nwazet.Commerce.Permissions {
    [OrchardFeature("Nwazet.WishLists")]
    public class WishListPermissions : IPermissionProvider {
        public static readonly Permission ViewWishLists = new Permission {
            Description = "View Wish Lists",
            Name = "ViewWishLists"
        };
        public static readonly Permission UpdateWishLists = new Permission {
            Description = "Update Wish Lists",
            Name = "UpdateWishLists"
        };
        public static readonly Permission DeleteWishLists = new Permission {
            Description = "Delete Wish Lists",
            Name = "DeleteWishLists"
        };


        public virtual Feature Feature { get; set; }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes() {
            return new[] {
                new PermissionStereotype {
                    Name = "Authenticated",
                    Permissions = new[] {
                        ViewWishLists,
                        UpdateWishLists,
                        DeleteWishLists }
                }
            };
        }

        public IEnumerable<Permission> GetPermissions() {
            return new[] {
                ViewWishLists,
                UpdateWishLists,
                DeleteWishLists
            };
        }
    }
}
