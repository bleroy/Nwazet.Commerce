using Nwazet.Commerce.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using System.Linq;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.AdvancedVAT")]
    public class VatConfigurationService : IVatConfigurationService {

        private readonly IContentManager _contentManager;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly ITerritoriesRepositoryService _territoriesRepositoryService;

        public VatConfigurationService(
            IContentManager contentManager,
            IWorkContextAccessor workContextAccessor,
            ITerritoriesRepositoryService territoriesRepositoryService) {

            _contentManager = contentManager;
            _workContextAccessor = workContextAccessor;
            _territoriesRepositoryService = territoriesRepositoryService;
        }

        private VatConfigurationSiteSettingsPart _settings { get; set; }

        private VatConfigurationSiteSettingsPart Settings {
            get {
                if (_settings == null) {
                    _settings = _workContextAccessor.GetContext().CurrentSite.As<VatConfigurationSiteSettingsPart>();
                }
                return _settings;
            }
        }

        // We save the Id of the VatConfigurationPart for the default TaxProductCategory in
        // the site settings. This way it will always be available.
        // If that value is ==0 we know that no VatConfigurationPart has ever been set as default.
        // Deletion of the default VatConfigurationPart will have to be prevented elsewhere.
        // Deletion can be prevented by using a dynamic permission, but only in those cases where
        // the attempt to delete comes through a method that actually checks for those. For
        // example, admin/contents/remove is fine.

        public int GetDefaultCategoryId() {
            return Settings.DefaultVatConfigurationId;
        }

        public void SetDefaultCategory(VatConfigurationPart part) {
            if (part.ContentItem.Id != GetDefaultCategoryId()) {
                // the part is not the default yet
                Settings.DefaultVatConfigurationId = part.ContentItem.Id;
            }
        }

        public VatConfigurationPart GetDefaultCategory() {

            var id = GetDefaultCategoryId();
            if (id > 0) {
                return _contentManager.Get<VatConfigurationPart>(id);
            } else {
                // no default category set
                return null;
            }
        }

        public decimal GetRate(ProductPart part) {
            // Given the VatConfiguration for this product, 
            // find the rate for the front end
            if (Settings.DefaultTerritoryForVatId == 0) {
                // Do not add tax for front end, i.e. the price shown on front end is "before tax"
                return 0;
            }

            var defaultTerritory = _territoriesRepositoryService
                .GetTerritoryInternal(Settings.DefaultTerritoryForVatId);

            if (defaultTerritory == null) {
                // This is an error condition that may be caused by setting a territory as default, and
                // then deleting the territory without updating the configuration.
                return 0;
            }

            return GetRate(part, defaultTerritory);
        }

        public decimal GetRate(ProductPart part, TerritoryInternalRecord destination) {
            if (destination == null) {
                return GetRate(part);
            }

            var vatConfig = GetVatConfiguration(part);

            if (vatConfig == null) {
                // No vat configuration exists
                return 0;
            }

            var hierarchyConfigs = vatConfig
                .Hierarchies
                ?.Where(tup => {
                    var thp = tup.Item1;
                    return thp
                        .Record
                        .Territories
                        .Any(tpr => tpr.TerritoryInternalRecord != null
                            && tpr.TerritoryInternalRecord.Id == destination.Id);
                });

            if (hierarchyConfigs == null || !hierarchyConfigs.Any()) {
                // territory is not in the hierarchies, so use the default rate
                return vatConfig.DefaultRate / 100.0m;
            }

            // get the territory exception if it exists
            var territoryConfig = vatConfig
                .Territories
                ?.Where(tup => {
                    var tp = tup.Item1;
                    return tp
                        .Record
                        .TerritoryInternalRecord
                        .Id == destination.Id;
                });
            if (territoryConfig == null || !territoryConfig.Any()) {
                // see if the default territory is a child of a territory with a configured
                // rate
                territoryConfig = vatConfig
                    .Territories
                    ?.Where(tup => {
                        var tp = tup.Item1;
                        var children = tp.Children;
                        var isChild = false;
                        while (children != null && children.Any()) {
                            isChild = children // search through the children
                                .Any(ci => {
                                    var territory = ci.As<TerritoryPart>();
                                    return territory != null //sanity check
                                        && territory.Record.TerritoryInternalRecord.Id == destination.Id;
                                });
                            if (isChild) {
                                break;
                            }
                            // then we search through the children's children
                            children = children
                                .Where(ci => ci.As<TerritoryPart>() != null) //sanity chedk
                                .SelectMany(ci => ci.As<TerritoryPart>().Children);
                        }
                        return isChild;
                    });
            }

            if (territoryConfig == null || !territoryConfig.Any()) {
                // there is no territory-specific configuration

                // We handle the error case where we have multiple territories satisfying the query by
                // sending the minimum of the rates. If there is only a single configuration for hierarchies
                // (the correct case) the following instruction will return the only rate.
                return hierarchyConfigs.Select(tup => tup.Item2).Min() / 100.0m;
            }

            // We handle the error case where we have multiple territories satisfying the query by
            // sending the minimum of the rates. If there is only a single configuration for territories
            // (the correct case) the following instruction will return the only rate.
            return territoryConfig.Select(tup => tup.Item2).Min() / 100.0m;

            // the way this method is written, having a configuration specific for a territory "fixes" the 
            // error condition where the territory is ni more than one hierarchy
        }

        public TerritoryInternalRecord GetDefaultDestination() {
            if (Settings.DefaultTerritoryForVatId == 0) {
                return null;
            }

            return _territoriesRepositoryService
                .GetTerritoryInternal(Settings.DefaultTerritoryForVatId);
        }


        private VatConfigurationPart GetVatConfiguration(ProductPart part) {
            return part
                ?.As<ProductVatConfigurationPart>()
                ?.VatConfigurationPart
                ?? GetDefaultCategory();
        }


    }
}
