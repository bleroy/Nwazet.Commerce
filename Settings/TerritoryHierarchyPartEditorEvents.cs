using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nwazet.Commerce.Settings {
    [OrchardFeature("Territories")]
    public class TerritoryHierarchyPartEditorEvents : ContentDefinitionEditorEventsBase {

        private readonly ITerritoriesService _territoriesService;

        public TerritoryHierarchyPartEditorEvents(
            ITerritoriesService territoriesService) {

            _territoriesService = territoriesService;

            T = NullLocalizer.Instance;
        }

        public Localizer T;

        public override IEnumerable<TemplateViewModel> TypePartEditor(ContentTypePartDefinition definition) {
            if (definition.PartDefinition.Name != TerritoryHierarchyPart.PartName) {
                yield break;
            }

            var settings = definition.Settings.GetModel<TerritoryHierarchyPartSettings>();
            if (string.IsNullOrWhiteSpace(settings.TerritoryType)) {
                settings.TerritoryType = _territoriesService.GetTerritoryTypes().FirstOrDefault()?.Name;
            }
            yield return DefinitionTemplate(ViewModel(settings));
        }

        public override IEnumerable<TemplateViewModel> TypePartEditorUpdate(ContentTypePartDefinitionBuilder builder, IUpdateModel updateModel) {
            if (builder.Name != TerritoryHierarchyPart.PartName) {
                yield break;
            }

            var vm = new TerritoryHierarchyPartSettingsViewModel();
            if (updateModel.TryUpdateModel(vm, "TerritoryHierarchyPartSettingsViewModel", null, null)) {
                var settings = vm.Settings;
                if (!_territoriesService.GetTerritoryTypes().Select(tt => tt.Name).Contains(settings.TerritoryType)) {
                    // we are not allowed to manage the type we are trying to assign so error things out
                    updateModel.AddModelError("TerritoryType", T("The type {0} is not allowed.", settings.TerritoryType));
                } else {
                    builder
                        .WithSetting("TerritoryHierarchyPartSettings.TerritoryType",
                            settings.TerritoryType);
                }
                builder
                    .WithSetting("TerritoryHierarchyPartSettings.MayChangeTerritoryTypeOnItem", 
                        settings.MayChangeTerritoryTypeOnItem.ToString(CultureInfo.InvariantCulture));
            }
        }

        private TerritoryHierarchyPartSettingsViewModel ViewModel(TerritoryHierarchyPartSettings part) {
            // Using ITerritoriesService.GetTerritoryTypes() here rather than going through IContentDefinitionManager
            // should already provide filtering by authorization, so that if I cannot manage a given type, I am not
            // allowed to use it in a hierarchy.
            return new TerritoryHierarchyPartSettingsViewModel {
                Settings = part,
                TerritoryTypes = _territoriesService
                    .GetTerritoryTypes()
                    .ToDictionary(tt => tt.Name, tt => tt.DisplayName)
            };
        }
    }
}
