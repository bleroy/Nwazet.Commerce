using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using Orchard.Localization;

namespace Nwazet.Commerce.Handlers {
    [OrchardFeature("Stripe")]
    public class StripeSettingsPartHandler : ContentHandler {
        public StripeSettingsPartHandler() {
            T = NullLocalizer.Instance;
            Filters.Add(new ActivatingFilter<StripeSettingsPart>("Site"));
        }

        public Localizer T { get; set; }

        protected override void GetItemMetadata(GetContentItemMetadataContext context) {
            if (context.ContentItem.ContentType != "Site")
                return;
            base.GetItemMetadata(context);
            context.Metadata.EditorGroupInfo.Add(new GroupInfo(T("Stripe")) {
                Id = "Stripe",
                Position = "4.1"
            });
        }
    }
}