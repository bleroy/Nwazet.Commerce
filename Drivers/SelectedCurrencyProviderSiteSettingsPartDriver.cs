using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Controllers;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;

namespace Nwazet.Commerce.Drivers {
    public class SelectedCurrencyProviderSiteSettingsPartDriver : ContentPartDriver<SelectedCurrencyProviderSiteSettingsPart> {

        private readonly IEnumerable<ISelectedCurrencyProvider> _selectedCurrencyProviders;
        protected override string Prefix
        {
            get { return "SelectedCurrencyProviderSiteSettings"; }
        }

        public SelectedCurrencyProviderSiteSettingsPartDriver(IEnumerable<ISelectedCurrencyProvider> selectedCurrencyProviders) {
            _selectedCurrencyProviders = selectedCurrencyProviders;
        }

        protected override DriverResult Editor(SelectedCurrencyProviderSiteSettingsPart part, dynamic shapeHelper) {
            return ContentShape("SiteSettings_Selected_Currency_Provider",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "SiteSettings/SelectedCurrencyProvidersList",
                    Model: new SelectedCurrencyProviderSiteSettingsViewModel() { Providers = 
                        _selectedCurrencyProviders.Select(scp => {
                            if (scp.Name == part.ActiveProvider) {
                                scp.Active = true;
                            }
                            else {
                                scp.Active = false;
                            }
                            return scp;
                        }).ToList()},
                    Prefix: Prefix))
                .OnGroup("ECommerceSiteSettings");
        }

        protected override DriverResult Editor(SelectedCurrencyProviderSiteSettingsPart part, IUpdateModel updater, dynamic shapeHelper) {
            var model = new SelectedCurrencyProviderSiteSettingsViewModel();
            if (updater is ECommerceSettingsAdminController &&
                updater.TryUpdateModel(model, Prefix, null, null)) {
                if (model.Providers.Count == 1) {
                    part.ActiveProvider = model.Providers.First().Name;
                }
                else {
                    var activeProvider = model.Providers.FirstOrDefault(scp => scp.Active);
                    part.ActiveProvider = activeProvider != null ? activeProvider.Name : string.Empty;
                }
            }
            return Editor(part, shapeHelper);
        }
    }
}
