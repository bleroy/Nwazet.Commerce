using Nwazet.Commerce;
using Nwazet.Commerce.Permissions;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Nwazet.Commerce.Menus {
    [OrchardFeature("Nwazet.Orders")]
    public class OrderAdminMenu : INavigationProvider {
        public string MenuName {
            get { return "admin"; }
        }

        public OrderAdminMenu() {
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
                        .Caption(T("Orders"))
                        .Position("2.1")
                        .Action("List", "OrderAdmin", new { area = "Nwazet.Commerce" })
                        .Permission(OrderPermissions.ViewOwnOrders)
                    )
                );
        }
    }
}