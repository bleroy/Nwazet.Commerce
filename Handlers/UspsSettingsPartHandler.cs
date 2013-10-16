using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using Orchard.Localization;

namespace Nwazet.Commerce.Handlers {
    [OrchardFeature("Usps.Shipping")]
    public class UspsSettingsPartHandler : ContentHandler {
        public UspsSettingsPartHandler() {
            T = NullLocalizer.Instance;
            Filters.Add(new ActivatingFilter<UspsSettingsPart>("Site"));
        }

        public Localizer T { get; set; }

        protected override void GetItemMetadata(GetContentItemMetadataContext context) {
            if (context.ContentItem.ContentType != "Site")
                return;
            base.GetItemMetadata(context);
            context.Metadata.EditorGroupInfo.Add(new GroupInfo(T("USPS")) {
                Id = "Usps",
                Position = "4.3"
            });
        }
    }
}