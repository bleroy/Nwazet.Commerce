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
                .Add(item => item
                    .Caption(T("Settings"))
                    .Add(subItem => subItem
                        .Caption(T("E-Commerce"))
                        .Position("2.1")
                        .Action("Index", "ECommerceSettingsAdmin", new { area = "Nwazet.Commerce" })
                        .Permission(CommercePermissions.ManageCommerce)
                    )
                );
        }
    }
}
