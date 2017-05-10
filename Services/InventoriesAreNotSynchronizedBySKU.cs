using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Nwazet.Commerce.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.UI.Admin.Notification;
using Orchard.UI.Notify;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.InventoryBySKU")]
    public class InventoriesAreNotSynchronizedBySKU : INotificationProvider {
        private readonly IWorkContextAccessor _workContextAccessor;
        public InventoriesAreNotSynchronizedBySKU(IWorkContextAccessor workContextAccessor) {

            _workContextAccessor = workContextAccessor;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public IEnumerable<NotifyEntry> GetNotifications() {
            var workContext = _workContextAccessor.GetContext();
            var settings = workContext.CurrentSite.As<InventoryBySKUSiteSettingsPart>();
            if (!settings.InventoriesAreAllInSynch) {
                var urlHelper = new UrlHelper(workContext.HttpContext.Request.RequestContext);
                var url = urlHelper.Action("Index", "ECommerceSettingsAdmin", new { Area = "Nwazet.Commerce" });
                yield return new NotifyEntry {
                    Message = T("Some inventories may be out of synch. Verify this from the <a href=\"{0}\">ecommerce settings</a>", url),
                    Type = NotifyType.Error
                };
            }
        }
    }
}
