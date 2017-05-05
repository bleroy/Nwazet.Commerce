using System;
using System.Linq;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.Settings;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Localization.Models;
using Orchard.Localization.Services;
using Orchard.UI.Notify;

namespace Nwazet.Commerce.Handlers {
    [OrchardFeature("Nwazet.AttributesLocalizationExtension")]
    public class LocalizationAttributePartHandler : ContentHandler {
        private readonly IProductAttributeService _attributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IOrchardServices _orchardServices;
        private readonly IContentManager _contentManager;
        private readonly IProductAttributeLocalizationServices _productAttributeLocalizationServices;
        public LocalizationAttributePartHandler(
            IProductAttributeService attributeService,
            ILocalizationService localizationService,
            IOrchardServices orchardServices,
            IContentManager contentManager,
            IProductAttributeLocalizationServices productAttributeLocalizationServices) {

            _attributeService = attributeService;
            _localizationService = localizationService;
            _orchardServices = orchardServices;
            _contentManager = contentManager;
            _productAttributeLocalizationServices = productAttributeLocalizationServices;

            T = NullLocalizer.Instance;
        }

        public Localizer T;

        private string DisplayTextFromId(int id) {
            return _contentManager.GetItemMetadata(_contentManager.GetLatest(id)).DisplayText;
        }

        protected override void UpdateEditorShape(UpdateEditorContext context) {
            base.UpdateEditorShape(context);

            //Here we implement localization logic
            LocalizationPart locPart = context.Content.As<LocalizationPart>();
            ProductAttributesPart attributesPart = context.ContentItem.As<ProductAttributesPart>();
            if (locPart != null && attributesPart != null) {
                if (attributesPart.AttributeIds.Count() > 0) {
                    var settings = attributesPart.TypePartDefinition.Settings.GetModel<ProductAttributeLocalizationSettings>();

                    if (settings.TryToLocalizeAttributes) {
                        //try to replace attributes with their correct localization
                        //newAttributesIds is IEnumerable<AttributeIdPair>.
                        //newAttributesIds.OriginalId is the attribute id in the initial ProductAttributesPart
                        //newAttributesIds.NewId is the attribute id after localization (<0 if no localization is found)
                        var newAttributeIds = _productAttributeLocalizationServices.GetLocalizationIdPairs(attributesPart, locPart);

                        if (newAttributeIds.Any(ni => ni.NewId < 0)) {
                            if (settings.RemoveAttributesWithoutLocalization) {
                                //remove the items for which we could not find a localization
                                _orchardServices.Notifier.Warning(T(
                                    "We could not find a correct localization for the following attributes, so they were removed from this product: {0}",
                                    string.Join(", ", newAttributeIds.Where(ni => ni.NewId < 0)
                                        .Select(ni => DisplayTextFromId(ni.OriginalId)
                                        )
                                    )
                                ));
                                newAttributeIds = newAttributeIds.Where(tup => tup.NewId > 0);
                            }
                            else {
                                //negative Ids are made positive again
                                newAttributeIds = newAttributeIds.Select(ni => ni = new AttributeIdPair(ni.OriginalId, Math.Abs(ni.NewId)));
                            }
                        }
                        //replace the ids
                        attributesPart.AttributeIds = newAttributeIds.Select(ni => ni.NewId).Distinct();
                        if (newAttributeIds.Where(ni => ni.OriginalId != ni.NewId).Any()) {
                            _orchardServices.Notifier.Warning(T(
                                   "The following attributes where replaced by their correct localization: {0}",
                                   string.Join(", ", newAttributeIds.Where(ni => ni.OriginalId != ni.NewId)
                                       .Select(ni => DisplayTextFromId(ni.OriginalId)
                                       )
                                   )
                               ));
                        }
                    }

                    if (settings.AssertAttributesHaveSameCulture) {
                        //verify that all the attributes are in the same culture as the product
                        var badAttributes = _productAttributeLocalizationServices.GetAttributesInTheWrongCulture(attributesPart, locPart);
                        if (badAttributes.Any()) {
                            context.Updater.AddModelError("",
                                T("Some of the attributes have the wrong culture: {0}",
                                string.Join(", ", badAttributes.Select(ba => _contentManager.GetItemMetadata(ba).DisplayText))
                                ));
                        }
                    }
                }
            }
        }
    }
}
