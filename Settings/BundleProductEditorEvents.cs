using System;
using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;

namespace Nwazet.Commerce.Settings {
    public class BundleProductEditorEvents : ContentDefinitionEditorEventsBase {
        public override IEnumerable<TemplateViewModel> TypePartEditor(ContentTypePartDefinition definition) {
            if (definition.PartDefinition.Name != "BundlePart") {
                yield break;
            }
            var settings = definition.Settings.GetModel<BundleProductSettings>();
            yield return DefinitionTemplate(settings);
        }
        public override IEnumerable<TemplateViewModel> TypePartEditorUpdate(ContentTypePartDefinitionBuilder builder, IUpdateModel updateModel) {
            if (builder.Name != "BundlePart") {
                yield break;
            }
            var settings = new BundleProductSettings();
            if (updateModel.TryUpdateModel(settings, "BundleProductSettings", null, null)) {
                builder
                    .WithSetting("BundleProductSettings.Autocomplete",
                     ((Boolean)settings.Autocomplete).ToString());
            }
            yield return DefinitionTemplate(settings);
        }
    }
}