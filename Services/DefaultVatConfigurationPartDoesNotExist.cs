using Nwazet.Commerce.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.UI.Admin.Notification;
using Orchard.UI.Notify;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.AdvancedVAT")]
    public class DefaultVatConfigurationPartDoesNotExist : INotificationProvider {
        
        private readonly IWorkContextAccessor _workContextAccessor;

        public DefaultVatConfigurationPartDoesNotExist(
            IWorkContextAccessor workContextAccessor) {

            _workContextAccessor = workContextAccessor;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public IEnumerable<NotifyEntry> GetNotifications() {
            var workContext = _workContextAccessor.GetContext();
            // The following comparison is not the same as saying <=0, because the
            // first term is nullable
            if (!(workContext.CurrentSite
                ?.As<VatConfigurationSiteSettingsPart>()
                ?.DefaultVatConfigurationId > 0)) {

                var url = new UrlHelper(workContext.HttpContext.Request.RequestContext)
                    .Action("Index", "TaxAdmin", new { Area = "Nwazet.Commerce" });

                yield return new NotifyEntry {
                    Message = T("No default tax product category found. Have an administrator configure one <a href=\"{0}\">here</a>.", url),
                    Type = NotifyType.Error
                };
            }
        }
    }
}
