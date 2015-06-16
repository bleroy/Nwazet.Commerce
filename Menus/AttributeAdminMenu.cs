using Nwazet.Commerce.Permissions;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Navigation;

namespace Nwazet.Commerce.Menus {
    [OrchardFeature("Nwazet.Attributes")]
    public class AttributeAdminMenu : INavigationProvider {
        public string MenuName {
            get { return "admin"; }
        }

        public AttributeAdminMenu() {
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
                        .Caption(T("Attributes"))
                        .Position("2.4")
                        .Action("Index", "AttributesAdmin", new { area = "Nwazet.Commerce" })
                        .Permission(CommercePermissions.CommerceAdministrator)
                    )
                );
        }
    }
}
