using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Settings;
using Orchard.UI.Navigation;

namespace Nwazet.Commerce.Menus {
    [OrchardSuppressDependency("Orchard.Core.Settings.AdminMenu")]
    public class AdminMenu : INavigationProvider {
        private readonly ISiteService _siteService;

        public AdminMenu(ISiteService siteService, IOrchardServices orchardServices) {
            _siteService = siteService;
            Services = orchardServices;
        }

        public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }
        public IOrchardServices Services { get; private set; }

        public void GetNavigation(NavigationBuilder builder) {
            builder.AddImageSet("settings")
                .Add(T("Settings"), "99",
                    menu => menu.Add(T("General"), "0", item => item.Action("Index", "Admin", new { area = "Settings", groupInfoId = "Index" })
                        .Permission(StandardPermissions.SiteOwner)), new [] {"collapsed"});

            var site = _siteService.GetSiteSettings();
            if (site == null)
                return;

            foreach (var groupInfo in Services.ContentManager.GetEditorGroupInfos(site.ContentItem)) {
                GroupInfo info = groupInfo;
                // The pricing menu is displayed in the Commerce section so skip adding it to the Settings section
                if (groupInfo.Id != "Pricing") { 
                    builder.Add(T("Settings"),
                        menu => menu.Add(info.Name, info.Position, item => item.Action("Index", "Admin", new { area = "Settings", groupInfoId = info.Id })
                            .Permission(StandardPermissions.SiteOwner)));
                }
            }
        }
    }
}
