using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.Settings;
using Nwazet.Commerce.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;
using Orchard.Localization.Models;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Nwazet.AttributesLocalizationExtension")]
    public class AttributeLocalizationProductAttributesPartDriver : ContentPartDriver<ProductAttributesPart>, IProductAttributesDriver {
        private readonly IProductAttributeService _attributeService;

        public AttributeLocalizationProductAttributesPartDriver(
            IProductAttributeService attributeService) {

            _attributeService = attributeService;
        }

        protected override string Prefix { get { return "NwazetCommerceAttribute"; } }

        public dynamic GetAttributeDisplayShape(IContent product, dynamic shapeHelper) {
            return null;
        }

        public bool ValidateAttributes(IContent product, IDictionary<int, ProductAttributeValueExtended> attributeIdsToValues) {
            return true;
        }

        protected override DriverResult Editor(ProductAttributesPart part, dynamic shapeHelper) {
            //check the settings
            List<ProductAttributePart> badAttributes = new List<ProductAttributePart>();
            if (part.TypePartDefinition.Settings.GetModel<ProductAttributeLocalizationSettings>().HideAttributesFromEditor) {
                LocalizationPart locPart = part.ContentItem.As<LocalizationPart>();
                if (locPart != null && locPart.Culture != null && !string.IsNullOrWhiteSpace(locPart.Culture.Culture)) {
                    //We need to have selected a product's culture
                    //check the attributes and tell to the view which ones to hide away
                    badAttributes.AddRange(_attributeService.Attributes.Where(pap => {
                        var lP = pap.ContentItem.As<LocalizationPart>();
                        return lP != null && //has a LocalizationPart AND
                            (lP.Culture == null || //culture undefined OR
                                (string.IsNullOrWhiteSpace(lP.Culture.Culture) || //culture undefined OR
                                    (lP.Culture != locPart.Culture))); //culture different than the product's 
                    }));
                }
            }
            return ContentShape(
                "Parts_ProductAttributes_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/AttributeLocalizationProductAttributes",
                    Prefix: Prefix,
                    Model: new ProductAttributesPartEditViewModel {
                        Prefix = Prefix,
                        Part = part,
                        Attributes = badAttributes
                    }));
        }
    }
}
