using Nwazet.Commerce.Helpers;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Usps.Shipping")]
    public class UspsSettingsPartDriver : ContentPartDriver<UspsSettingsPart> {
        protected override string Prefix {
            get { return "UspsSettings"; }
        }

        protected override DriverResult Editor(UspsSettingsPart part, dynamic shapeHelper) {
            return ContentShape("Parts_Usps_Settings",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/UspsSettings",
                    Model: part,
                    Prefix: Prefix)).OnGroup("USPS");
        }

        protected override DriverResult Editor(UspsSettingsPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }

        protected override void Importing(UspsSettingsPart part, ImportContentContext context) {
            var el = context.Data.Element(typeof (UspsSettingsPart).Name);
            if (el == null) return;
            el.With(part)
                .FromAttr(p => p.UserId)
                .FromAttr(p => p.OriginZip)
                .FromAttr(p => p.CommercialPrices)
                .FromAttr(p => p.CommercialPlusPrices);
        }

        protected override void Exporting(UspsSettingsPart part, ExportContentContext context) {
            context.Element(typeof (UspsSettingsPart).Name)
                .With(part)
                .ToAttr(p => p.UserId)
                .ToAttr(p => p.OriginZip)
                .ToAttr(p => p.CommercialPrices)
                .ToAttr(p => p.CommercialPlusPrices);
        }
    }
}