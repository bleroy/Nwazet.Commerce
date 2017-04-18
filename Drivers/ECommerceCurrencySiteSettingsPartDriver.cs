using Nwazet.Commerce.Controllers;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Nwazet.CurrencyProviderBySiteSetting")]
    public class ECommerceCurrencySiteSettingsPartDriver : ContentPartDriver<ECommerceCurrencySiteSettingsPart> {

        private readonly ICurrencyProvider _currencyProvider;


        public ECommerceCurrencySiteSettingsPartDriver(
            ICurrencyProvider currencyProvider) {

            _currencyProvider = currencyProvider;
        }

        protected override string Prefix
        {
            get { return "ECommerceCurrencySiteSettings"; }
        }
        
        protected override DriverResult Editor(ECommerceCurrencySiteSettingsPart part, dynamic shapeHelper) {
            if (_currencyProvider is UseCurrencyFromSiteSettingsProvider) {
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
            if (_currencyProvider is UseCurrencyFromSiteSettingsProvider) {
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
