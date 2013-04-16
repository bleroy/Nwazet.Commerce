using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Nwazet.Commerce.Menus {
    [OrchardFeature("Nwazet.Shipping")]
    public class ShippingAdminMenu : INavigationProvider {
        public string MenuName {
            get { return "admin"; }
        }

        public ShippingAdminMenu() {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void GetNavigation(NavigationBuilder builder) {
            builder
                .AddImageSet("nwazet-commerce")
                .Add(item => item
                    .Caption(T("Commerce"))
                    .Position("2")
                    .LinkToFirstChild(true)

                    .Add(subItem => subItem
                        .Caption(T("Shipping Methods"))
                        .Position("2.3")
                        .Action("Index", "ShippingAdmin", new { area = "Nwazet.Commerce" })
                    )
                );
        }
    }
}
