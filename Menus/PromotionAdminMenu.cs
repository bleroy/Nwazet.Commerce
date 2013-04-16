using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Nwazet.Commerce.Menus {
    [OrchardFeature("Nwazet.Promotions")]
    public class PromotionAdminMenu : INavigationProvider {
        public string MenuName {
            get { return "admin"; }
        }

        public PromotionAdminMenu() {
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
                        .Caption(T("Promotions"))
                        .Position("2.4")
                        .Action("Index", "PromotionAdmin", new { area = "Nwazet.Commerce" })
                    )
                );
        }
    }
}
