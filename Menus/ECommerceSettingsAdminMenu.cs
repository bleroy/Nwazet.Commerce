using Nwazet.Commerce.Permissions;
using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Nwazet.Commerce.Menus {
    public class ECommerceSettingsAdminMenu : INavigationProvider {

        public string MenuName
        {
            get { return "admin"; }
        }

        public ECommerceSettingsAdminMenu() {
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
                        .Caption(T("Settings"))
                        .Position("2.1")
                        .Action("Index", "ECommerceSettingsAdmin", new { area = "Nwazet.Commerce" })
                        .Permission(CommercePermissions.ManageCommerce)
                    )
                );
        }
    }
}
