using Nwazet.Commerce.Permissions;
using Nwazet.Commerce.Services;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Nwazet.Commerce.Menus {
    [OrchardFeature("Territories")]
    public class TerritoriesAdminMenu : INavigationProvider {

        private readonly ITerritoriesService _territoriesService;
        private readonly ITerritoriesPermissionProvider _permissionProvider;

        public TerritoriesAdminMenu(
            ITerritoriesService territoriesService,
            ITerritoriesPermissionProvider permissionProvider) {

            _territoriesService = territoriesService;
            _permissionProvider = permissionProvider;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public string MenuName {
            get { return "admin"; }
        }


        public void GetNavigation(NavigationBuilder builder) {
            builder.Add(item => item
                .Caption(T("Territories"))
                .Position("3")
                .LinkToFirstChild(false)
                
                .Add(subItem => {
                    subItem = subItem
                        .Caption(T("Hierarchies"))
                        .Position("1")
                        .Action("Index", "TerritoryHierarchiesAdmin", new { area = "Nwazet.Commerce" })
                        .Permission(TerritoriesPermissions.ManageTerritoryHierarchies);
                    foreach (var permission in _permissionProvider.ListHierarchyTypePermissions()) {
                        subItem = subItem.Permission(permission);
                    }
                })

                .Add(subItem =>  subItem
                    .Caption(T("Territories"))
                    .Position("2")
                    .Action("TerritoriesIndex", "TerritoriesAdmin", new { area = "Nwazet.Commerce" })
                    .Permission(TerritoriesPermissions.ManageInternalTerritories)
                )
            );
        }
    }
}
