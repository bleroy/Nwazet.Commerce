using System;
using Nwazet.Commerce.Models;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Google.Checkout")]
    public class GoogleCheckoutSettingsPartDriver : ContentPartDriver<GoogleCheckoutSettingsPart> {
        private readonly ISignals _signals;

        public GoogleCheckoutSettingsPartDriver(ISignals signals) {
            _signals = signals;
        }

        protected override string Prefix { get { return "GoogleCheckoutSettings"; } }

        protected override DriverResult Editor(GoogleCheckoutSettingsPart part, dynamic shapeHelper) {
            return ContentShape("Parts_GoogleCheckout_Settings",
                               () => shapeHelper.EditorTemplate(
                                   TemplateName: "Parts/GoogleCheckoutSettings",
                                   Model: part.Record,
                                   Prefix: Prefix)).OnGroup("GoogleCheckout");
        }

        protected override DriverResult Editor(GoogleCheckoutSettingsPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part.Record, Prefix, null, null);
            _signals.Trigger("GoogleCheckout.Changed");
            return Editor(part, shapeHelper);
        }

        protected override void Importing(GoogleCheckoutSettingsPart part, ImportContentContext context) {
            var merchantId = context.Attribute(part.PartDefinition.Name, "MerchantId");
            if (!String.IsNullOrWhiteSpace(merchantId)) {
                part.MerchantId = merchantId;
            }
            var currency = context.Attribute(part.PartDefinition.Name, "Currency");
            if (!String.IsNullOrWhiteSpace(currency)) {
                part.Currency = currency;
            }
            var weightUnit = context.Attribute(part.PartDefinition.Name, "WeightUnit");
            if (!String.IsNullOrWhiteSpace(weightUnit)) {
                part.WeightUnit = weightUnit;
            }
            var analyticsId = context.Attribute(part.PartDefinition.Name, "AnalyticsId");
            if (!String.IsNullOrWhiteSpace(analyticsId)) {
                part.AnalyticsId = analyticsId;
            }
            var useSandboxAttribute = context.Attribute(part.PartDefinition.Name, "UseSandbox");
            bool useSandbox;
            if (Boolean.TryParse(useSandboxAttribute, out useSandbox)) {
                part.UseSandbox = useSandbox;
            }
        }

        protected override void Exporting(GoogleCheckoutSettingsPart part, ExportContentContext context) {
            context.Element(part.PartDefinition.Name).SetAttributeValue("MerchantId", part.MerchantId);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Currency", part.Currency);
            context.Element(part.PartDefinition.Name).SetAttributeValue("WeightUnit", part.WeightUnit);
            context.Element(part.PartDefinition.Name).SetAttributeValue("AnalyticsId", part.AnalyticsId);
            context.Element(part.PartDefinition.Name).SetAttributeValue("UseSandbox", part.UseSandbox.ToString().ToLower());
        }
    }
}
