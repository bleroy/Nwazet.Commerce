using System;
using Nwazet.Commerce.Models;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Stripe")]
    public class StripeSettingsPartDriver : ContentPartDriver<StripeSettingsPart> {
        private readonly ISignals _signals;

        public StripeSettingsPartDriver(ISignals signals) {
            _signals = signals;
        }

        protected override string Prefix { get { return "StripeSettings"; } }

        protected override DriverResult Editor(StripeSettingsPart part, dynamic shapeHelper) {
            return ContentShape("Parts_Stripe_Settings",
                               () => shapeHelper.EditorTemplate(
                                   TemplateName: "Parts/StripeSettings",
                                   Model: part.Record,
                                   Prefix: Prefix)).OnGroup("Stripe");
        }

        protected override DriverResult Editor(StripeSettingsPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part.Record, Prefix, null, null);
            _signals.Trigger("Stripe.Changed");
            return Editor(part, shapeHelper);
        }

        protected override void Importing(StripeSettingsPart part, ImportContentContext context) {
            var publishableKey = context.Attribute(part.PartDefinition.Name, "PublishableKey");
            if (!String.IsNullOrWhiteSpace(publishableKey)) {
                part.PublishableKey = publishableKey;
            }
        }

        protected override void Exporting(StripeSettingsPart part, ExportContentContext context) {
            context
                .Element(part.PartDefinition.Name)
                .SetAttributeValue("PublishableKey", part.PublishableKey);
        }
    }
}
