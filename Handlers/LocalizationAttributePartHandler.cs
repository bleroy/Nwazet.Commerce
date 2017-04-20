using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Localization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Nwazet.Commerce.Settings;
using Nwazet.Commerce.Services;
using Orchard.Localization.Services;
using Orchard;
using Orchard.UI.Notify;

namespace Nwazet.Commerce.Handlers {
    [OrchardFeature("Nwazet.AttributesLocalizationExtension")]
    public class LocalizationAttributePartHandler : ContentHandler {
        private readonly IProductAttributeService _attributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IOrchardServices _orchardServices;
        private readonly IContentManager _contentManager;
        public LocalizationAttributePartHandler(
            IProductAttributeService attributeService,
            ILocalizationService localizationService,
            IOrchardServices orchardServices,
            IContentManager contentManager) {

            _attributeService = attributeService;
            _localizationService = localizationService;
            _orchardServices = orchardServices;
            _contentManager = contentManager;

            T = NullLocalizer.Instance;
        }

        public Localizer T;
        protected override void GetItemMetadata(GetContentItemMetadataContext context) {
            var part = context.ContentItem.As<ProductAttributePart>();
            if (part != null) {
                var locPart = context.ContentItem.As<LocalizationPart>();
                if (locPart != null) {
                    if (locPart.Culture != null && !string.IsNullOrWhiteSpace(locPart.Culture.Culture)) {
                        context.Metadata.DisplayText += T(" (culture: {0})", locPart.Culture.Culture).Text;
                    }
                    else {
                        context.Metadata.DisplayText += T(" (culture undefined)").Text;
                    }
                }
                if (context.ContentItem.HasDraft() && !context.ContentItem.IsPublished()) {
                    //ContentItem is draft
                    context.Metadata.DisplayText += T(" (draft)").Text;
                }
            }
        }

        protected override void UpdateEditorShape(UpdateEditorContext context) {
            base.UpdateEditorShape(context);

            //Here we implement localization logic
            LocalizationPart locPart = context.Content.As<LocalizationPart>();
            ProductAttributesPart attributesPart = context.ContentItem.As<ProductAttributesPart>();
            if (locPart != null && attributesPart != null) {
                if (attributesPart.AttributeIds.Count() > 0) {
                    var settings = attributesPart.PartDefinition.Settings.GetModel<ProductAttributeLocalizationSettings>();

                    var initialAttributes = _attributeService.GetAttributes(attributesPart.AttributeIds);

                    if (settings.TryToLocalizeAttributes) {
                        //try to replace attributes with their correct localization
                        //newAttributesIds is IEnumerable<Tuple<int, int>>.
                        //newAttributesIds.Item1 is the attribute id in the initial ProductAttributesPart
                        //newAttributesIds.Item2 is the attribute id after localization
                        var newAttributeIds = initialAttributes.Select(pap => {
                            var ci = pap.ContentItem;
                            if (_localizationService.GetContentCulture(ci) == locPart.Culture.Culture) {
                                //this attribute is fine
                                return new Tuple<int, int>(ci.Id, ci.Id);
                            }
                            var localized = _localizationService.GetLocalizations(ci)
                                .FirstOrDefault(lp => lp.Culture == locPart.Culture);
                            return localized == null ?
                                new Tuple<int, int>(ci.Id, -ci.Id) : //negative id where we found no localization
                                new Tuple<int, int>(ci.Id, localized.Id);
                        });

                        if (newAttributeIds.Any(ni => ni.Item2 < 0)) {
                            if (settings.RemoveAttributesWithoutLocalization) {
                                //remove the items for which we could not find a localization
                                _orchardServices.Notifier.Warning(T(
                                    "We could not find a correct localization for the following attributes, so they were removed: {1}",
                                    string.Join(", ", newAttributeIds.Where(ni => ni.Item2 < 0)
                                        .Select(tup => _contentManager.GetItemMetadata(
                                            _contentManager.GetLatest(tup.Item1)).DisplayText
                                        )
                                    )
                                ));
                                newAttributeIds = newAttributeIds.Where(tup => tup.Item2 > 0);
                            }
                            else {
                                //negative Ids are made positive again
                                newAttributeIds = newAttributeIds.Select(tup => tup = new Tuple<int, int>(tup.Item1, Math.Abs(tup.Item2)));
                            }
                        }

                    }
                }
            }
        }
    }
}
