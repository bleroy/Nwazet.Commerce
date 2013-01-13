using Orchard.Environment.Extensions;
using Orchard.Localization;
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

        private Localizer T { get; set; }

        public void GetNavigation(NavigationBuilder builder) {
            builder
                .AddImageSet("nwazet-commerce")
                .Add(item => item
                    .Caption(T("Commerce"))
                    .Position("2")
                    .LinkToFirstChild(true)

                    .Add(subItem => subItem
                        .Caption(T("Attributes"))
                        .Position("2.2")
                        .Action("Index", "AttributesAdmin", new { area = "Nwazet.Commerce" })
                    )
                );
        }
    }
}
