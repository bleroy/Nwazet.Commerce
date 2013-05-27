using System;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Usps.Shipping")]
    public class UspsSettingsPartDriver : ContentPartDriver<UspsSettingsPart>
    {
        protected override string Prefix { get { return "UspsSettings"; } }

        protected override DriverResult Editor(UspsSettingsPart part, dynamic shapeHelper) {
            return ContentShape("Parts_Usps_Settings",
                               () => shapeHelper.EditorTemplate(
                                   TemplateName: "Parts/UspsSettings",
                                   Model: part.Record,
                                   Prefix: Prefix)).OnGroup("USPS");
        }

        protected override DriverResult Editor(UspsSettingsPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part.Record, Prefix, null, null);
            return Editor(part, shapeHelper);
        }

        protected override void Importing(UspsSettingsPart part, ImportContentContext context)
        {
            var userId = context.Attribute(part.PartDefinition.Name, "UserId");
            if (!String.IsNullOrWhiteSpace(userId)) {
                part.UserId = userId;
            }
            var originZip = context.Attribute(part.PartDefinition.Name, "OriginZip");
            if (!String.IsNullOrWhiteSpace(originZip)) {
                part.OriginZip = originZip;
            }
            var commercialPrices = context.Attribute(part.PartDefinition.Name, "CommercialPrices");
            bool useCommercialPrices;
            if (Boolean.TryParse(commercialPrices, out useCommercialPrices)) {
                part.CommercialPrices = useCommercialPrices;
            }
            var commercialPlusPrices = context.Attribute(part.PartDefinition.Name, "CommercialPlusPrices");
            bool useCommercialPlusPrices;
            if (Boolean.TryParse(commercialPlusPrices, out useCommercialPlusPrices))
            {
                part.CommercialPlusPrices = useCommercialPlusPrices;
            }
        }

        protected override void Exporting(UspsSettingsPart part, ExportContentContext context)
        {
            context.Element(part.PartDefinition.Name).SetAttributeValue("UserId", part.UserId);
            context.Element(part.PartDefinition.Name).SetAttributeValue("OriginZip", part.OriginZip);
            context.Element(part.PartDefinition.Name).SetAttributeValue("CommercialPrices", part.CommercialPrices.ToString().ToLower());
            context.Element(part.PartDefinition.Name).SetAttributeValue("CommercialPlusPrices", part.CommercialPlusPrices.ToString().ToLower());
        }
    }
}
