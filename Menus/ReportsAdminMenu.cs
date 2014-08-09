using Nwazet.Commerce.Permissions;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Nwazet.Commerce.Menus {
    [OrchardFeature("Nwazet.Reports")]
    public class ReportsAdminMenu : INavigationProvider {
        public string MenuName {
            get { return "admin"; }
        }

        public ReportsAdminMenu() {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void GetNavigation(NavigationBuilder builder) {
            builder
                .AddImageSet("nwazet-commerce")
                .Add(item => item
                    .Caption(T("Commerce"))
                    .Position("2")
                    .LinkToFirstChild(false)

                    .Add(subItem => subItem
                        .Caption(T("Reports"))
                        .Position("2.8")
                        .Action("Index", "Report", new { area = "Nwazet.Commerce" })
                        .Permission(OrderPermissions.ManageOrders)
                    )
                );
        }
    }
}
