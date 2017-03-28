using System.Collections.Generic;
using Orchard.Environment.Extensions;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace Nwazet.Commerce.Permissions {
    [OrchardFeature("Nwazet.Commerce")]
    public class CommercePermissions : IPermissionProvider {
        public static readonly Permission ManageCommerce = new Permission {
            Description = "Manage Commerce",
            Name = "ManageCommerce"
        };

        public static readonly Permission ManageTaxes = new Permission {
            Description = "Manage Taxes",
            Name = "ManageTaxes",
            ImpliedBy = new[] { ManageCommerce }
        };

        public static readonly Permission ManageShipping = new Permission {
            Description = "Manage Shipping",
            Name = "ManageShipping",
            ImpliedBy = new[] { ManageCommerce }
        };

        public static readonly Permission ManagePromotions = new Permission {
            Description = "Manage Promotions",
            Name = "ManagePromotions",
            ImpliedBy = new[] { ManageCommerce }
        };

        public static readonly Permission ManagePricing = new Permission {
            Description = "Manage Pricing",
            Name = "ManagePricing",
            ImpliedBy = new[] { ManageCommerce }
        };

        public static readonly Permission ManageProducts = new Permission {
            Description = "Manage Products",
            Name = "ManageProducts",
            ImpliedBy = new[] { ManageCommerce }
        };

        public static readonly Permission ManageAttributes = new Permission {
            Description = "Manage Attributes",
            Name = "ManageAttributes",
            ImpliedBy = new[] { ManageCommerce }
        };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions() {
            return new[] {
                ManageCommerce,
                ManageTaxes,
                ManageShipping,
                ManagePromotions,
                ManagePricing,
                ManageProducts,
                ManageAttributes
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes() {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] { ManageCommerce, ManageTaxes, ManageShipping, ManagePromotions, ManagePricing, ManageProducts, ManageAttributes }
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
