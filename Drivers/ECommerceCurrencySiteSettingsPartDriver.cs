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
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;

namespace Nwazet.Commerce.Drivers {
   public class ECommerceCurrencySiteSettingsPartDriver : ContentPartDriver<ECommerceCurrencySiteSettingsPart> {

        private readonly ISelectedCurrencyProvider _selectedCurrencyProvider;


        public ECommerceCurrencySiteSettingsPartDriver(ISelectedCurrencyProvider selectedCurrencyProvider) {
            _selectedCurrencyProvider = selectedCurrencyProvider;
        }

        protected override string Prefix
        {
            get { return "ECommerceCurrencySiteSettings"; }
        }
        
        protected override DriverResult Editor(ECommerceCurrencySiteSettingsPart part, dynamic shapeHelper) {
            if (_selectedCurrencyProvider is UseCurrencyFromSiteSettingsProvider) {
                return ContentShape("SiteSettings_Currency",
                    () => shapeHelper.EditorTemplate(
                        TemplateName: "SiteSettings/Currency",
                        Model: new ECommerceCurrencySiteSettingsViewModel() { CurrencyCode = part.CurrencyCode },
                        Prefix: Prefix))
                    .OnGroup("ECommerceSiteSettings");
            }
            return null;
        }

        protected override DriverResult Editor(ECommerceCurrencySiteSettingsPart part, IUpdateModel updater, dynamic shapeHelper) {
            if (_selectedCurrencyProvider is UseCurrencyFromSiteSettingsProvider) {
                var model = new ECommerceCurrencySiteSettingsViewModel();
                if (updater is ECommerceSettingsAdminController &&
                    updater.TryUpdateModel(model, Prefix, null, null)) {
                    part.CurrencyCode = model.CurrencyCode;
                }
            }
            return Editor(part, shapeHelper);
        }
    }
}
