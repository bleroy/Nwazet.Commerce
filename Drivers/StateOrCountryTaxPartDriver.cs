using Nwazet.Commerce.Helpers;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Nwazet.Taxes")]
    public class StateOrCountryPartDriver : ContentPartDriver<StateOrCountryTaxPart> {

        protected override string Prefix {
            get { return "NwazetCommerceStateOrCountryTax"; }
        }

        protected override DriverResult Display(
            StateOrCountryTaxPart part, string displayType, dynamic shapeHelper) {
            return ContentShape(
                "Parts_StateOrCountryTax",
                () => shapeHelper.Parts_StateOrCountryTax(
                    Name: part.Name,
                    Country: part.Country,
                    State: part.State,
                    Rate: part.Rate,
                    Priority: part.Priority,
                    ContentItem: part.ContentItem));
        }

        //GET
        protected override DriverResult Editor(StateOrCountryTaxPart part, dynamic shapeHelper) {
            return ContentShape("Parts_StateOrCountryTax_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/StateOrCountryTax",
                    Model: shapeHelper.StateOrCountryTaxEditor(
                        Tax: part,
                        Prefix: Prefix),
                    Prefix: Prefix));
        }

        //POST
        protected override DriverResult Editor(StateOrCountryTaxPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            part.Rate = part.Rate/100; // User will input a percentage
            return Editor(part, shapeHelper);
        }

        protected override void Importing(StateOrCountryTaxPart part, ImportContentContext context) {
            var el = context.Data.Element(typeof (StateOrCountryTaxPart).Name);
            if (el == null) return;
            el.With(part)
                .FromAttr(p => p.Country)
                .FromAttr(p => p.State)
                .FromAttr(p => p.Rate)
                .FromAttr(p => p.Priority);
        }

        protected override void Exporting(StateOrCountryTaxPart part, ExportContentContext context) {
            context.Element(typeof (StateOrCountryTaxPart).Name)
                .With(part)
                .ToAttr(p => p.Country)
                .ToAttr(p => p.State)
                .ToAttr(p => p.Rate)
                .ToAttr(p => p.Priority);
        }
    }
}