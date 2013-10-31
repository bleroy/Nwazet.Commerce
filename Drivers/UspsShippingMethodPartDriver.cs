using System.Linq;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Usps.Shipping")]
    public class UspsShippingMethodPartDriver : ContentPartDriver<UspsShippingMethodPart> {
        private readonly IUspsService _uspsService;

        public UspsShippingMethodPartDriver(IUspsService uspsService) {
            _uspsService = uspsService;
        }

        protected override string Prefix {
            get { return "NwazetCommerceUspsShipping"; }
        }

        protected override DriverResult Display(
            UspsShippingMethodPart part, string displayType, dynamic shapeHelper) {
            return ContentShape(
                "Parts_UspsShippingMethod",
                () => {
                    var internationalAreas = _uspsService.GetInternationalShippingAreas().ToList();
                    var domesticAreas = _uspsService.GetDomesticShippingAreas().ToList();
                    var included = part.International ? internationalAreas : domesticAreas;
                    var excluded = part.International ? domesticAreas : internationalAreas;
                    return shapeHelper.Parts_UspsShippingMethod(
                        Name: part.Name,
                        ShippingCompany: part.ShippingCompany,
                        IncludedShippingAreas: included,
                        ExcludedShippingAreas: excluded,
                        ContentItem: part.ContentItem);
                });
        }

        //GET
        protected override DriverResult Editor(UspsShippingMethodPart part, dynamic shapeHelper) {
            return Combined(ContentShape("Parts_UspsShippingMethod_Edit",
                                         () => shapeHelper.EditorTemplate(
                                             TemplateName: "Parts/UspsShippingMethod",
                                             Model: part,
                                             Prefix: Prefix)),
                            ContentShape("UspsShippingTestForm",
                                         () => {
                                             var settings = _uspsService.GetSettings();
                                             return shapeHelper.UspsShippingTestForm(
                                                 ShippingMethod: part,
                                                 UspsSettings: settings);
                                         }));
        }

        //POST
        protected override DriverResult Editor(UspsShippingMethodPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }

        protected override void Importing(UspsShippingMethodPart part, ImportContentContext context) {
            var el = context.Data.Element(typeof (UspsShippingMethodPart).Name);
            if (el == null) return;
            el.With(part)
              .FromAttr(p => p.Name)
              .FromAttr(p => p.Size)
              .FromAttr(p => p.WidthInInches)
              .FromAttr(p => p.LengthInInches)
              .FromAttr(p => p.HeightInInches)
              .FromAttr(p => p.MaximumWeightInOunces)
              .FromAttr(p => p.Priority)
              .FromAttr(p => p.International)
              .FromAttr(p => p.RegisteredMail)
              .FromAttr(p => p.Insurance)
              .FromAttr(p => p.ReturnReceipt)
              .FromAttr(p => p.CertificateOfMailing)
              .FromAttr(p => p.ElectronicConfirmation);
        }

        protected override void Exporting(UspsShippingMethodPart part, ExportContentContext context) {
            context.Element(typeof (UspsShippingMethodPart).Name)
                   .With(part)
                   .ToAttr(p => p.Name)
                   .ToAttr(p => p.Size)
                   .ToAttr(p => p.WidthInInches)
                   .ToAttr(p => p.LengthInInches)
                   .ToAttr(p => p.HeightInInches)
                   .ToAttr(p => p.MaximumWeightInOunces)
                   .ToAttr(p => p.Priority)
                   .ToAttr(p => p.International)
                   .ToAttr(p => p.RegisteredMail)
                   .ToAttr(p => p.Insurance)
                   .ToAttr(p => p.ReturnReceipt)
                   .ToAttr(p => p.CertificateOfMailing)
                   .ToAttr(p => p.ElectronicConfirmation);
        }
    }
}
