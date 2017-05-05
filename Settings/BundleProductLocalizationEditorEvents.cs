using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Settings {
    [OrchardFeature("Nwazet.BundlesLocalizationExtension")]
    public class BundleProductLocalizationEditorEvents : ContentDefinitionEditorEventsBase {

        private readonly IContentDefinitionManager _contentDefinitionManager;
        public BundleProductLocalizationEditorEvents(IContentDefinitionManager contentDefinitionManager) {
            _contentDefinitionManager = contentDefinitionManager;
        }

        private bool ShouldProcessSettings(ContentTypeDefinition definition) {
            bool hasLocalizationPart = definition
                .Parts.Any(ctpd => ctpd.PartDefinition.Name == "LocalizationPart");

            return hasLocalizationPart;
        }

        public override IEnumerable<TemplateViewModel> TypePartEditor(ContentTypePartDefinition definition) {
            if (definition.PartDefinition.Name != "BundlePart") {
                yield break;
            }

            if (ShouldProcessSettings(definition.ContentTypeDefinition)) {
                var settings = definition.Settings.GetModel<BundleProductLocalizationSettings>();
                yield return DefinitionTemplate(settings);
            }
        }

        public override IEnumerable<TemplateViewModel> TypePartEditorUpdate(ContentTypePartDefinitionBuilder builder, IUpdateModel updateModel) {
            if (builder.Name != "BundlePart") {
                yield break;
            }

            if (ShouldProcessSettings(_contentDefinitionManager.GetTypeDefinition(builder.TypeName))) {
                var settings = new BundleProductLocalizationSettings();
                if (updateModel.TryUpdateModel(settings, "BundleProductLocalizationSettings", null, null)) {
                    builder
                        .WithSetting("BundleProductLocalizationSettings.TryToLocalizeProducts",
                            settings.TryToLocalizeProducts.ToString(CultureInfo.InvariantCulture));
                    builder
                        .WithSetting("BundleProductLocalizationSettings.RemoveProductsWithoutLocalization",
                            settings.RemoveProductsWithoutLocalization.ToString(CultureInfo.InvariantCulture));
                    builder
                        .WithSetting("BundleProductLocalizationSettings.HideProductsFromEditor",
                            settings.AssertProductsHaveSameCulture.ToString(CultureInfo.InvariantCulture));
                    builder
                        .WithSetting("BundleProductLocalizationSettings.HideAttributesFromEditor",
                            settings.HideProductsFromEditor.ToString(CultureInfo.InvariantCulture));
                }
            }
        }
    }
}
