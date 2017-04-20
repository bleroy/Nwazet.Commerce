using Nwazet.Commerce.Permissions;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Navigation;

namespace Nwazet.Commerce.Menus {
    [OrchardFeature("Nwazet.Taxes")]
    public class TaxAdminMenu : INavigationProvider {
        public string MenuName {
            get { return "admin"; }
        }

        public TaxAdminMenu() {
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
                        .Caption(T("Taxes"))
                        .Position("2.6")
                        .Action("Index", "TaxAdmin", new {area = "Nwazet.Commerce"})
                        .Permission(CommercePermissions.ManageTaxes)
                    )
                );
        }
    }
}
