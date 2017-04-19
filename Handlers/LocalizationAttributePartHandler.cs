using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Localization.Models;

namespace Nwazet.Commerce.Handlers {
    [OrchardFeature("Nwazet.AttributesLocalizationExtension")]
    public class LocalizationAttributePartHandler : ContentHandler {
        public LocalizationAttributePartHandler() {
            T = NullLocalizer.Instance;
        }

        public Localizer T;
        protected override void GetItemMetadata(GetContentItemMetadataContext context) {
            var part = context.ContentItem.As<ProductAttributePart>();
            if (part != null) {
                var locPart = context.ContentItem.As<LocalizationPart>();
                if (locPart != null) {
                    if (locPart.Culture != null && !string.IsNullOrWhiteSpace(locPart.Culture.Culture)) {
                        context.Metadata.DisplayText += T(" (culture: {0})", locPart.Culture.Culture).Text;
                    }
                    else {
                        context.Metadata.DisplayText += T(" (culture undefined)").Text;
                    }
                }
                if (context.ContentItem.HasDraft() && !context.ContentItem.IsPublished()) {
                    //ContentItem is draft
                    context.Metadata.DisplayText += T(" (draft)").Text;
                }
            }
        }
    }
}
