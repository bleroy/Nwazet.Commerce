using Orchard.Environment.Extensions;
using Orchard.Localization;
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

        private Localizer T { get; set; }

        public void GetNavigation(NavigationBuilder builder) {
            builder
                .AddImageSet("nwazet-commerce")
                .Add(item => item
                    .Caption(T("Commerce"))
                    .Position("2")
                    .LinkToFirstChild(true)

                    .Add(subItem => subItem
                        .Caption(T("Products"))
                        .Position("2.0")
                        .Action("List", "ProductAdmin", new { area = "Nwazet.Commerce" })
                    )
                );
        }
    }
}
