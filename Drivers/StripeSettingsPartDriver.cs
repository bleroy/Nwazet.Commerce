using System;
using Nwazet.Commerce.Helpers;
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

        protected override string Prefix {
            get { return "StripeSettings"; }
        }

        protected override DriverResult Editor(StripeSettingsPart part, dynamic shapeHelper) {
            return ContentShape("Parts_Stripe_Settings",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/StripeSettings",
                    Model: part,
                    Prefix: Prefix)).OnGroup("Stripe");
        }

        protected override DriverResult Editor(StripeSettingsPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            _signals.Trigger("Stripe.Changed");
            return Editor(part, shapeHelper);
        }

        protected override void Importing(StripeSettingsPart part, ImportContentContext context) {
            var el = context.Data.Element(typeof (StripeSettingsPart).Name);
            if (el == null) return;
            el.With(part)
                .FromAttr(p => p.PublishableKey)
                .FromAttr(p => p.SecretKey)
                .FromAttr(p => p.Currency);
        }

        protected override void Exporting(StripeSettingsPart part, ExportContentContext context) {
            context
                .Element(typeof (StripeSettingsPart).Name)
                .With(part)
                .ToAttr(p => p.PublishableKey)
                .ToAttr(p => p.SecretKey)
                .ToAttr(p => p.Currency);
        }
    }
}