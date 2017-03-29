using Nwazet.Commerce.Permissions;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Navigation;

namespace Nwazet.Commerce.Menus {
    [OrchardFeature("Nwazet.Commerce")]
    public class ProductAdminMenu : INavigationProvider {
        public string MenuName {
            get { return "admin"; }
        }

        public ProductAdminMenu() {
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
                        .Caption(T("Products"))
                        .Position("2.3")
                        .Action("List", "ProductAdmin", new { area = "Nwazet.Commerce" })
                        .Permission(CommercePermissions.ManageProducts)
                    )
                );
        }
    }
}
