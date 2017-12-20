using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.Settings;
using Nwazet.Commerce.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.UI.Notify;
using System.Collections.Generic;
using System.Linq;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Territories")]
    public class TerritoryHierarchyPartDriver : ContentPartDriver<TerritoryHierarchyPart> {

        private ITerritoriesService _territoriesService;
        private readonly INotifier _notifier;

        public TerritoryHierarchyPartDriver(
            ITerritoriesService territoriesService,
            INotifier notifier) {

            _territoriesService = territoriesService;
            _notifier = notifier;

            T = NullLocalizer.Instance;
        }

        public Localizer T;

        protected override string Prefix {
            get { return "TerritoryHierarchyPart"; }
        }

        protected override DriverResult Display(TerritoryHierarchyPart part, string displayType, dynamic shapeHelper) {
            return Combined(
                ContentShape("Parts_TerritoryHierarchy_SummaryAdmin",
                    () => shapeHelper.Parts_TerritoryHierarchy_SummaryAdmin(TerritoriesCount: part.Territories.Count())
                    ));
        }

        protected override DriverResult Editor(TerritoryHierarchyPart part, dynamic shapeHelper) {
            var shapes = new List<DriverResult>();
            //part.Id == 0: new item
            if (part.Id != 0) {
                //if the part is fully configured add a shape that allows managing the territories in the hierarchy
                if (!string.IsNullOrWhiteSpace(part.TerritoryType)) {
                    if (_territoriesService.GetTerritoryTypes().Any(tt => tt.Name == part.TerritoryType)) {
                        // add the shape for the territories in the hierachy
                        shapes.Add(ContentShape("Parts_TerritoryHierarchy_TerritoryManager",
                            () => shapeHelper.EditorTemplate(
                                TemplateName: "Parts/TerritoryHierarchyTerritoryManager",
                                Model: new TerritoryHierarchyTerritoryManagerViewModel(part) {
                                    TopLevelCount = _territoriesService
                                        .GetTerritoriesQuery(part, null, VersionOptions.Latest)
                                        .Count()
                                },
                                Prefix: Prefix
                                )));
                    } else {
                        _notifier.Warning(T("You are not allowed to manage the territories for this hierarchy."));
                    }
                }
            }
            //add a shape for configuration of the part
            //Some configuration options may be locked depending on the territories in the hierarchy
            //e.g. once there are territories, we cannot change the TerritoryType anymore
            shapes.Add(ContentShape("Parts_TerritoryHierarchy_TerritoryTypeSelection",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/TerritoryHierarchyTerritoryTypeSelection",
                    Model: TypeSelectionVM(part),
                    Prefix: Prefix
                    )));
            return Combined(shapes.ToArray());
        }

        protected override DriverResult Editor(TerritoryHierarchyPart part, IUpdateModel updater, dynamic shapeHelper) {
            // Territories in the hierarchy are handled in a specific controller
            var typeSelectionVM = new TerritoryHierarchyTypeSelectionViewModel();
            if (updater.TryUpdateModel(typeSelectionVM, Prefix, null, null)) {
                if (typeSelectionVM.MayChangeTerritoryType) {
                    if (MayChangeTerritoryType(part)) {
                        part.TerritoryType = typeSelectionVM.TerritoryType;
                    } else if (part.TerritoryType != typeSelectionVM.TerritoryType) {
                        updater.AddModelError("TerritoryType", T("It became impossible to change the Type of the territories in this hierarchy."));
                    } else {
                        _notifier.Warning(T("It became impossible to change the Type of the territories in this hierarchy."));
                    }
                }
            }

            return Editor(part, shapeHelper);
        }

        private bool MayChangeTerritoryType(TerritoryHierarchyPart part) {
            return !(part.Territories?.Any() ?? false) &&
                part.Settings.GetModel<TerritoryHierarchyPartSettings>().MayChangeTerritoryTypeOnItem;
        }

        private TerritoryHierarchyTypeSelectionViewModel TypeSelectionVM(TerritoryHierarchyPart part) {
            return new TerritoryHierarchyTypeSelectionViewModel {
                Part = part,
                TerritoryType = part.TerritoryType,
                TerritoryTypes = _territoriesService
                    .GetTerritoryTypes()
                    .ToDictionary(tt => tt.Name, tt => tt.DisplayName),
                MayChangeTerritoryType = MayChangeTerritoryType(part)
            };
        }

        protected override void Exporting(TerritoryHierarchyPart part, ExportContentContext context) {
            context.Element(part.PartDefinition.Name).SetAttributeValue("TerritoryType", part.TerritoryType);
        }

        protected override void Importing(TerritoryHierarchyPart part, ImportContentContext context) {
            if (context.Data.Element(part.PartDefinition.Name) == null) {
                return;
            }

            part.TerritoryType = context.Attribute(part.PartDefinition.Name, "TerritoryType");
        }
    }
}
