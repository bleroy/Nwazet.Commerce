using Nwazet.Commerce.Models;
using Nwazet.Commerce.Permissions;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Security;
using System.Globalization;
using System.Linq;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Nwazet.AdvancedVAT")]
    public class TerritoryVatConfigurationPartDriver : ContentPartDriver<TerritoryVatConfigurationPart> {

        private readonly IAuthorizer _authorizer;
        private readonly IVatConfigurationProvider _vatConfigurationProvider;
        private readonly IContentManager _contentManager;
        private readonly IWorkContextAccessor _workContextAccessor;

        public TerritoryVatConfigurationPartDriver(
            IAuthorizer authorizer,
            IVatConfigurationProvider vatConfigurationProvider,
            IContentManager contentManager,
            IWorkContextAccessor workContextAccessor) {

            _authorizer = authorizer;
            _vatConfigurationProvider = vatConfigurationProvider;
            _contentManager = contentManager;
            _workContextAccessor = workContextAccessor;

            T = NullLocalizer.Instance;
        }

        public Localizer T;

        protected override string Prefix {
            get { return "TerritoryVatConfigurationPart"; }
        }

        private CultureInfo SiteCulture {
            get {
                return CultureInfo.GetCultureInfo(_workContextAccessor.GetContext().CurrentCulture);
            }
        }

        protected override DriverResult Editor(TerritoryVatConfigurationPart part, dynamic shapeHelper) {
            // We only need to display the VAT configurations that have been selected in the
            // Hierarchy
            var hierarchy = part.As<TerritoryPart>().HierarchyPart;
            if (hierarchy == null) {
                // this is the case for a new territory
                hierarchy = part.As<TerritoryPart>().CreationHierarchy;
            }
            if (hierarchy != null) { // this should always be true
                var configurations = hierarchy.As<HierarchyVatConfigurationPart>()?.VatConfigurations;
                var model = new TerritoryVatConfigurationPartViewModel {
                    AllVatConfigurations = configurations == null 
                        ? new VatConfigurationDetailViewModel[] { }
                        : configurations
                            .Select(cfg => {
                                var specificConfig = part.VatConfigurations
                                    ?.FirstOrDefault(tup => tup.Item1.Record == cfg.Item1.Record);
                                return new VatConfigurationDetailViewModel {
                                    VatConfigurationPartId = cfg.Item1.Record.Id,
                                    VatConfigurationPartText = cfg.Item1.TaxProductCategory,
                                    IsSelected = specificConfig != null,
                                    Rate = specificConfig == null
                                        ? 0
                                        : specificConfig.Item2,
                                    RateString = specificConfig == null
                                        ? "0"
                                        : specificConfig.Item2.ToString(SiteCulture)
                                };
                            })
                            .ToArray()
                };

                // two different views depending on the permission. If we cannot manage taxes we will still show
                // the configuration, aso as a warning for people trying to delete or edit the territory.
                var shapeName = "Parts_TerritoryVatConfiguration_Edit";
                var templateName = "Parts/TerritoryVatConfigurationPartNotAllowed";
                if (_authorizer.Authorize(CommercePermissions.ManageTaxes)) {
                    templateName = "Parts/TerritoryVatConfigurationPartAllowed";
                }

                return ContentShape(shapeName,
                () => shapeHelper.EditorTemplate(
                    TemplateName: templateName,
                    Model: model,
                    Prefix: Prefix
                    ));
            }
            return null;
        }

        protected override DriverResult Editor(TerritoryVatConfigurationPart part, IUpdateModel updater, dynamic shapeHelper) {
            if (_authorizer.Authorize(CommercePermissions.ManageTaxes)) {
                // update
                var model = new TerritoryVatConfigurationPartViewModel();
                if (updater.TryUpdateModel(model, Prefix, null, null)) {
                    // parse rates from strings or fail the update
                    bool success = true;
                    foreach (var vm in model.AllVatConfigurations.Where(c => c.IsSelected)) {
                        decimal d = 0;
                        if (!decimal.TryParse(vm.RateString, NumberStyles.Any, SiteCulture, out d)) {
                            success = false;
                            updater.AddModelError(T("Rate").Text, T("{0} Is not a valid value for rate.", vm.RateString));
                        } else {
                            vm.Rate = d;
                        }
                    }
                    if (success) {
                        _vatConfigurationProvider.UpdateConfiguration(part, model);
                    }
                }
            }

            return Editor(part, shapeHelper);
        }
    }
}
