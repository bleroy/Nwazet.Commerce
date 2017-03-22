using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Permissions;
using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Nwazet.Commerce.Menus {
    class ECommerceSettingsAdminMenu : INavigationProvider {

        public string MenuName
        {
            get { return "admin"; }
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
