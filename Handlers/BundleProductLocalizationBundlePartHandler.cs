using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.Settings;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Localization.Models;

namespace Nwazet.Commerce.Handlers {
    [OrchardFeature("Nwazet.BundlesLocalizationExtension")]
    public class BundleProductLocalizationBundlePartHandler : ContentHandler {

        private readonly IBundleProductLocalizationServices _bundleProductLocalizationServices;
        private readonly IContentManager _contentManager;
        public BundleProductLocalizationBundlePartHandler(
            IBundleProductLocalizationServices bundleProductLocalizationServices,
            IContentManager contentManager) {

            _bundleProductLocalizationServices = bundleProductLocalizationServices;
            _contentManager = contentManager;
            T = NullLocalizer.Instance;
        }

        public Localizer T;

        protected override void UpdateEditorShape(UpdateEditorContext context) {
            base.UpdateEditorShape(context);

            //Localization logic
            var bundlePart = context.ContentItem.As<BundlePart>();
            if (bundlePart != null && bundlePart.ProductIds.Any()) {
                var locPart = context.ContentItem.As<LocalizationPart>();
                if (_bundleProductLocalizationServices.ValidLocalizationPart(locPart)) {
                    var settings = bundlePart.TypePartDefinition.Settings.GetModel<BundleProductLocalizationSettings>();

                    
                    if (settings.AssertProductsHaveSameCulture) {
                        //verify that all products are in the same culture as the bundle
                        var badProducts = _bundleProductLocalizationServices.GetProductsInTheWrongCulture(bundlePart, locPart);
                        if (badProducts.Any()) {
                            context.Updater.AddModelError("",
                                T("Some of the products are in the wrong culture: {0}",
                                string.Join(", ", badProducts.Select(bp => _contentManager.GetItemMetadata(bp).DisplayText))
                                ));
                        }
                    }
                }
            }
        }

    }
}
