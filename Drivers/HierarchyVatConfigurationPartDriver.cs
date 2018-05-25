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
    public class HierarchyVatConfigurationPartDriver : ContentPartDriver<HierarchyVatConfigurationPart> {

        private readonly IAuthorizer _authorizer;
        private readonly IVatConfigurationProvider _vatConfigurationProvider;
        private readonly IContentManager _contentManager;
        private readonly IWorkContextAccessor _workContextAccessor;

        public HierarchyVatConfigurationPartDriver(
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
            get { return "HierarchyVatConfigurationPart"; }
        }

        private CultureInfo SiteCulture {
            get {
                return CultureInfo.GetCultureInfo(_workContextAccessor.GetContext().CurrentCulture);
            }
        }

        protected override DriverResult Editor(HierarchyVatConfigurationPart part, dynamic shapeHelper) {

            // Create the view models for every Vat category that can be configured
            var allConfigurations = _vatConfigurationProvider.GetVatConfigurations();
            var model = new HierarchyVatConfigurationPartViewModel {
                AllVatConfigurations = allConfigurations
                    .Select(cfg => {
                        var specificConfig = part.VatConfigurations
                            ?.FirstOrDefault(tup => tup.Item1.Record == cfg.Record);
                        return new VatConfigurationDetailViewModel {
                            VatConfigurationPartId = cfg.Record.Id,
                            VatConfigurationPartText = cfg.TaxProductCategory,
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
            // two different views depending on the permission
            var shapeName = "Parts_HierarchyVatConfiguration_Edit";
            var templateName = "Parts/HierarchyVatConfigurationPartNotAllowed";
            if (_authorizer.Authorize(CommercePermissions.ManageTaxes)) {
                // we can manage taxes
                templateName = "Parts/HierarchyVatConfigurationPartAllowed";
            } 
            //else {
            //    // we cannot manage taxes, but we will still show what has been configured. This serves
            //    // also as a warning for people trying to delete or edit the hierarchy.
            //}

            return ContentShape(shapeName,
                () => shapeHelper.EditorTemplate(
                    TemplateName: templateName,
                    Model: model,
                    Prefix: Prefix
                    ));
        }

        protected override DriverResult Editor(HierarchyVatConfigurationPart part, IUpdateModel updater, dynamic shapeHelper) {
            if (_authorizer.Authorize(CommercePermissions.ManageTaxes)) {
                // update
                var model = new HierarchyVatConfigurationPartViewModel();
                if (updater.TryUpdateModel(model, Prefix, null, null)) {
                    bool success = true;
                    foreach (var vm in model.AllVatConfigurations.Where(c => c.IsSelected)) {
                        // parse from the string or fail the update
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
