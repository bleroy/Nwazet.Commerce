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
    /// <summary>
    /// Here we will check whether the territory selected as default for VAT computations
    /// actually exists as a TerritoryInternalRecord in the system. It might have been deleted
    /// unknowingly.
    /// </summary>
    [OrchardFeature("Nwazet.AdvancedVAT")]
    public class DefaultVatTerritoryIsInvalid : INotificationProvider {

        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly ITerritoriesRepositoryService _territoriesRepositoryService;

        public DefaultVatTerritoryIsInvalid(
            IWorkContextAccessor workContextAccessor,
            ITerritoriesRepositoryService territoriesRepositoryService) {

            _workContextAccessor = workContextAccessor;
            _territoriesRepositoryService = territoriesRepositoryService;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public IEnumerable<NotifyEntry> GetNotifications() {
            var workContext = _workContextAccessor.GetContext();

            var territoryId = workContext.CurrentSite
                ?.As<VatConfigurationSiteSettingsPart>()
                ?.DefaultTerritoryForVatId;

            var success = territoryId.HasValue;
            success &= success ? territoryId.Value >= 0 : false;
            success &= success
                ? (territoryId.Value == 0 || _territoriesRepositoryService.GetTerritoryInternal(territoryId.Value) != null)
                : false;

            if (!success) {
                var url = new UrlHelper(workContext.HttpContext.Request.RequestContext)
                    .Action("Index", "ECommerceSettingsAdmi", new { Area = "Nwazet.Commerce" });

                yield return new NotifyEntry {
                    Message = T("There is an error in the configuration of the default territory for VAT. Have an administrator verify it <a href=\"{0}\">here</a>.", url),
                    Type = NotifyType.Error
                };
            }
        }
    }
}
