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
    [OrchardFeature("Nwazet.AttributesLocalizationExtension")]
    public class ProductAttributeLocalizationEditorEvents : ContentDefinitionEditorEventsBase {

        private readonly IContentDefinitionManager _contentDefinitionManager;
        public ProductAttributeLocalizationEditorEvents(IContentDefinitionManager contentDefinitionManager) {
            _contentDefinitionManager = contentDefinitionManager;
        }

        private bool ShouldProcessSettings(ContentTypeDefinition definition) {
            bool hasLocalizationPart = definition
                .Parts.Any(ctpd => ctpd.PartDefinition.Name == "LocalizationPart");

            return hasLocalizationPart;
        }

        public override IEnumerable<TemplateViewModel> TypePartEditor(ContentTypePartDefinition definition) {
            if (definition.PartDefinition.Name != "ProductAttributesPart") yield break;

            if (ShouldProcessSettings(definition.ContentTypeDefinition)) {
                var settings = definition.Settings.GetModel<ProductAttributeLocalizationSettings>();
                yield return DefinitionTemplate(settings);
            }
        }

        public override IEnumerable<TemplateViewModel> TypePartEditorUpdate(ContentTypePartDefinitionBuilder builder, IUpdateModel updateModel) {
            if (builder.Name != "ProductAttributesPart") yield break;

            if (ShouldProcessSettings(_contentDefinitionManager.GetTypeDefinition(builder.TypeName))) {
                var settings = new ProductAttributeLocalizationSettings();
                if (updateModel.TryUpdateModel(settings, "ProductAttributeLocalizationSettings", null, null)) {
                    builder
                        .WithSetting("ProductAttributeLocalizationSettings.TryToLocalizeAttributes",
                            settings.TryToLocalizeAttributes.ToString(CultureInfo.InvariantCulture));
                    builder
                        .WithSetting("ProductAttributeLocalizationSettings.RemoveAttributesWithoutLocalization",
                            settings.RemoveAttributesWithoutLocalization.ToString(CultureInfo.InvariantCulture));
                    builder
                        .WithSetting("ProductAttributeLocalizationSettings.AssertAttributesHaveSameCulture",
                            settings.AssertAttributesHaveSameCulture.ToString(CultureInfo.InvariantCulture));
                }
                yield return DefinitionTemplate(settings);
            }
        }
    }
}
