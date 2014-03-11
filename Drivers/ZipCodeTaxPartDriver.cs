using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Nwazet.Taxes")]
    public class ZipCodeTaxPartDriver : ContentPartDriver<ZipCodeTaxPart> {

        protected override string Prefix {
            get { return "NwazetCommerceUnitesStatesZipCodeTax"; }
        }

        //GET
        protected override DriverResult Editor(ZipCodeTaxPart part, dynamic shapeHelper) {
            return ContentShape("Parts_ZipCodeTax_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/ZipCodeTax",
                    Model: shapeHelper.ZipCodeTax(
                        Tax: part,
                        Prefix: Prefix),
                    Prefix: Prefix));
        }

        //POST
        protected override DriverResult Editor(ZipCodeTaxPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            // User will input a percentage
            return Editor(part, shapeHelper);
        }
    }
}