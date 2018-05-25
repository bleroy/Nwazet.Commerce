using Nwazet.Commerce.Controllers;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using System.Linq;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Nwazet.AdvancedVAT")]
    public class VatConfigurationSiteSettingsPartDriver : ContentPartDriver<VatConfigurationSiteSettingsPart> {

        private readonly IVatConfigurationService _vatConfigurationService;
        private readonly IContentManager _contentManager;
        private readonly ITerritoriesRepositoryService _territoriesRepositoryService;

        public VatConfigurationSiteSettingsPartDriver(
            IVatConfigurationService vatConfigurationService,
            IContentManager contentManager,
            ITerritoriesRepositoryService territoriesRepositoryService) {

            _vatConfigurationService = vatConfigurationService;
            _contentManager = contentManager;
            _territoriesRepositoryService = territoriesRepositoryService;

            T = NullLocalizer.Instance;
        }

        public Localizer T;

        protected override string Prefix {
            get { return "VatConfigurationSiteSettings"; }
        }

        protected override DriverResult Editor(VatConfigurationSiteSettingsPart part, dynamic shapeHelper) {
            return ContentShape("SiteSettings_VatConfiguration",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "SiteSettings/VatConfiguration",
                    Model: CreateVM(part),
                    Prefix: Prefix
                    )
                ).OnGroup("ECommerceSiteSettings");
        }

        protected override DriverResult Editor(VatConfigurationSiteSettingsPart part, IUpdateModel updater, dynamic shapeHelper) {
            var model = new VatConfigurationSiteSettingsPartViewModel();
            if (updater is ECommerceSettingsAdminController 
                && updater.TryUpdateModel(model, Prefix, null, null)) {

                part.DefaultTerritoryForVatId = model.DefaultTerritoryForVatId;
            }
            return Editor(part, shapeHelper);
        }

        private VatConfigurationSiteSettingsPartViewModel CreateVM(VatConfigurationSiteSettingsPart part) {
            return new VatConfigurationSiteSettingsPartViewModel(T("None").Text) {
                DefaultVatConfigurationPart = _vatConfigurationService
                    .GetDefaultCategory(),
                DefaultTerritoryForVatId = part.DefaultTerritoryForVatId,
                AvailableTerritoryInternalRecords = _territoriesRepositoryService
                    .GetTerritories()
                    .ToList()
            };
        }
    }
}
