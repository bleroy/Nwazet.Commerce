using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment;
using Orchard.Environment.Extensions.Models;

namespace Nwazet.Commerce.Features {
    public class CurrencyProviderBySiteSettingFeature : IFeatureEventHandler {

        private readonly IOrchardServices _orchardServices;
        public CurrencyProviderBySiteSettingFeature(
            IOrchardServices orchardServices) {

            _orchardServices = orchardServices;
        }

        public void Enabled(Feature feature) {
            if (feature.Descriptor.Id == "Nwazet.CurrencyProviderBySiteSetting") {
                //initialize the currency code using the site's culture
                _orchardServices.WorkContext.CurrentSite.As<ECommerceCurrencySiteSettingsPart>().CurrencyCode =
                    new RegionInfo(
                        CultureInfo.GetCultureInfo(
                            _orchardServices.WorkContext.CurrentSite.SiteCulture
                            ).LCID)
                        .ISOCurrencySymbol;
            }
        }

        #region Not used interface methods
        public void Disabled(Feature feature) {
        }

        public void Disabling(Feature feature) {
        }

        public void Enabling(Feature feature) {
        }

        public void Installed(Feature feature) {
        }

        public void Installing(Feature feature) {
        }

        public void Uninstalled(Feature feature) {
        }

        public void Uninstalling(Feature feature) {
        }
        #endregion
    }
}
