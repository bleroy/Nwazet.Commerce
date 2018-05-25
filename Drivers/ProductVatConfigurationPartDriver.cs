using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using System.Linq;
using Orchard.ContentManagement.Handlers;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Nwazet.AdvancedVAT")]
    public class ProductVatConfigurationPartDriver : ContentPartDriver<ProductVatConfigurationPart> {

        private readonly IVatConfigurationProvider _vatConfigurationProvider;
        private readonly IContentManager _contentManager;

        public ProductVatConfigurationPartDriver(
            IVatConfigurationProvider vatConfigurationProvider,
            IContentManager contentManager) {

            _vatConfigurationProvider = vatConfigurationProvider;
            _contentManager = contentManager;

            T = NullLocalizer.Instance;
        }


        public Localizer T;

        protected override string Prefix {
            get { return "ProductVatConfigurationPart"; }
        }

        protected override DriverResult Editor(ProductVatConfigurationPart part, dynamic shapeHelper) {
            var model = CreateVM(part);
            return ContentShape("Parts_ProductVatConfiguration_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/ProductVatConfiguration",
                    Model: model,
                    Prefix: Prefix
                    ));
        }

        protected override DriverResult Editor(ProductVatConfigurationPart part, IUpdateModel updater, dynamic shapeHelper) {
            var model = new ProductVatConfigurationPartEditorViewModel();
            if (updater.TryUpdateModel(model, Prefix, null, null)) {
                // set the vat category
                if (model.VatConfigurationId == 0) {
                    part.Record.VatConfiguration = null;
                } else if (model.VatConfigurationId > 0) {
                    part.Record.VatConfiguration = _contentManager
                        .Get(model.VatConfigurationId) // will be null if that ContentItem is not Published
                        ?.As<VatConfigurationPart>() // will be null if that ContentItem is not VatConfigurationPart
                        ?.Record;
                }
            }
            return Editor(part, shapeHelper);
        }

        private ProductVatConfigurationPartEditorViewModel CreateVM(
            ProductVatConfigurationPart part) {
            return new ProductVatConfigurationPartEditorViewModel(T("Default").Text) {
                VatConfigurationId = part.UseDefaultVatCategory
                    ? 0
                    : part.VatConfigurationPart.Record.Id,
                AllVatConfigurations = _vatConfigurationProvider
                    .GetVatConfigurations()
                    .ToList()
            };
        }

        // Exporting and Importing should only be used in cloning the Items this part is attached to, because
        // the part itself is in relation with things outside the ContentItem that may not be exported/imported
        // at the same time
        protected override void Exporting(ProductVatConfigurationPart part, ExportContentContext context) {
            var element = context.Element(part.PartDefinition.Name);
            if (part.Record.VatConfiguration != null) {
                element.SetAttributeValue("VatConfigurationIdentity", GetIdentity(part.Record.VatConfiguration.Id));
                // we also set the Id, in case when we are importing we cannot get the item form the session
                // by using the identity
                element.SetAttributeValue("VatConfigurationId", part.Record.VatConfiguration.Id.ToString());
            }
        }

        protected override void Importing(ProductVatConfigurationPart part, ImportContentContext context) {
            if (context.Data.Element(part.PartDefinition.Name) == null) {
                return;
            }

            VatConfigurationPartRecord importedVat = null;

            var vatIdentity = context.Attribute(part.PartDefinition.Name, "VatConfigurationIdentity");
            if (vatIdentity != null) {
                var vatCi = context.GetItemFromSession(vatIdentity);
                importedVat = vatCi?.As<VatConfigurationPart>()?.Record;
            }

            if (importedVat == null) {
                // try using the id
                var vatIdObj = context.Attribute(part.PartDefinition.Name, "VatConfigurationId");
                int vatId = 0;
                if (!string.IsNullOrWhiteSpace(vatIdObj)
                    && int.TryParse(vatIdObj, out vatId)) {
                    if (vatId > 0) {
                        importedVat = _contentManager.Get<VatConfigurationPart>(vatId)?.Record;
                    }
                }
            }

            part.Record.VatConfiguration = importedVat;
        }

        private string GetIdentity(int id) {
            var ci = _contentManager.Get(id, VersionOptions.Latest);
            return _contentManager.GetItemMetadata(ci).Identity.ToString();
        }
    }
}
