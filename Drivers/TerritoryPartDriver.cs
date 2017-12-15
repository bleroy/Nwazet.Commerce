using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;
using Orchard;
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
    public class TerritoryPartDriver : ContentPartDriver<TerritoryPart> {

        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly ITerritoriesService _territoriesService;
        private readonly INotifier _notifier;
        private readonly IContentManager _contentManager;
        private readonly ITerritoriesRepositoryService _territoriesRepositoryService;
        private readonly ITerritoriesHierarchyService _territoriesHierarchyService;

        public TerritoryPartDriver(
            IWorkContextAccessor workContextAccessor,
            ITerritoriesService territoriesService,
            INotifier notifier,
            IContentManager contentManager,
            ITerritoriesRepositoryService territoriesRepositoryService,
            ITerritoriesHierarchyService territoriesHierarchyService) {

            _workContextAccessor = workContextAccessor;
            _territoriesService = territoriesService;
            _notifier = notifier;
            _contentManager = contentManager;
            _territoriesRepositoryService = territoriesRepositoryService;
            _territoriesHierarchyService = territoriesHierarchyService;

            T = NullLocalizer.Instance;
        }

        public Localizer T;

        protected override string Prefix {
            get { return "TerritoryPart"; }
        }

        protected override DriverResult Editor(TerritoryPart part, dynamic shapeHelper) {
            
            var shapes = new List<DriverResult>();
            //part.id == 0: new item
            if (part.Id == 0 || part.Record.Hierarchy == null) {
                shapes.AddRange(CreationEditor(part, shapeHelper));
            } else {
                shapes.AddRange(ProperEditor(part, shapeHelper));
            }
            
            return Combined(shapes.ToArray());
        }

        private IEnumerable<DriverResult> CreationEditor(TerritoryPart part, dynamic shapeHelper) {
            var shapes = new List<DriverResult>();
            // We don't know the Hierarchy for this territory here, so we try to get it from
            // the CreationHierarchy property of the part. We will need it validate a list of
            // allowed TerritoryInternalRecord
            var hierarchy = part.CreationHierarchy;
            if (hierarchy == null) {
                // We don't really have a hierarchy after all
                InvalidHierarchyOnCreation();
            } else {
                // Healthy situation
                var territoryInternals = _territoriesService.GetAvailableTerritoryInternals(hierarchy).ToList();
                if (territoryInternals.Any()) {
                    // There are TerritoryInternalRecords we can pick from
                    shapes.Add(ContentShape("Parts_TerritoryPart_Creation",
                        () => shapeHelper.EditorTemplate(
                            TemplateName: "Parts/TerritoryPartCreation",
                            Model: new TerritoryPartViewModel() {
                                AvailableTerritoryInternalRecords = territoryInternals,
                                Hierarchy = hierarchy
                            },
                            Prefix: Prefix
                            )));
                } else {
                    // There is no TerritoryInternalRecord available
                    // This is also verified in the HierarchyTerritoriesAdminController call. However, something
                    // has clearly happened in the meanwhile.
                    _notifier.Error(T("There are no territories that may be added to hierarchy. Content creation will fail."));
                }
            }
            

            return shapes;
        }

        private void InvalidHierarchyOnCreation() {
            InvalidHierarchyNotification(T("Content creation"));
        }

        private IEnumerable<DriverResult> ProperEditor(TerritoryPart part, dynamic shapeHelper) {

            var shapes = new List<DriverResult>();

            // The territory here must exist in a hierarchy and with a selected unique record.
            var territoryInternals = _territoriesService
                .GetAvailableTerritoryInternals(part.HierarchyPart)
                .ToList();
            var model = new TerritoryPartViewModel() {
                AvailableTerritoryInternalRecords = territoryInternals,
                Hierarchy = part.HierarchyPart,
                Parent = part.ParentPart,
                Part = part
            };
            if (part.Record.TerritoryInternalRecord != null) {
                model.AvailableTerritoryInternalRecords.Add(part.Record.TerritoryInternalRecord);
                model.SelectedRecordId = part.Record.TerritoryInternalRecord.Id.ToString();
            }

            shapes.Add(ContentShape("Parts_TerritoryPart_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/TerritoryPartEdit",
                    Model: model,
                    Prefix: Prefix
                    )));

            return shapes;
        }

        private void InvalidHierarchyOnEdit() {
            InvalidHierarchyNotification(T("Content edit"));
        }

        private void InvalidHierarchyNotification(LocalizedString detail) {
            _notifier.Error(InvalidHierarchyErrorMessage(detail));
        }

        private LocalizedString InvalidHierarchyErrorMessage(LocalizedString detail = null) {
            if (detail != null) {
                return T("Impossible to identify a valid Hierarchy for this territory. {0} will fail.", detail);
            }
            return T("Impossible to identify a valid Hierarchy for this territory.");
        }

        protected override DriverResult Editor(TerritoryPart part, IUpdateModel updater, dynamic shapeHelper) {

            var viewModel = new TerritoryPartViewModel();
            if (updater.TryUpdateModel(viewModel, Prefix, null, null)) {
                var hierarchy = part.HierarchyPart ?? part.CreationHierarchy;
                if (hierarchy == null) {
                    updater.AddModelError("Hierarchy", InvalidHierarchyErrorMessage());
                } else {
                    var avalaibleInternals = _territoriesService.GetAvailableTerritoryInternals(hierarchy);
                    int selectedId;
                    if (int.TryParse(viewModel.SelectedRecordId, out selectedId)) {
                        var selectedRecord = _territoriesRepositoryService.GetTerritoryInternal(selectedId);
                        if (selectedRecord == null) {
                            updater.AddModelError("Territory", InvalidInternalRecordMessage);
                        } else {
                            if (part.Record.TerritoryInternalRecord != null && part.Record.TerritoryInternalRecord.Id == selectedId) {
                                // nothing to do here, right?
                            } else {
                                var fromAvailables = avalaibleInternals.FirstOrDefault(tir => tir.Id == selectedId);
                                if (fromAvailables == null) {
                                    updater.AddModelError("Territory", InvalidInternalRecordMessage);
                                } else {
                                    _territoriesHierarchyService.AssignInternalRecord(part, selectedId);
                                }
                            }
                        }
                    }
                }

            }

            return Editor(part, shapeHelper);
        }

        private LocalizedString InvalidInternalRecordMessage {
            get { return T("Invalid territory record."); }
        }
        
        protected override void Exporting(TerritoryPart part, ExportContentContext context) {
            // we set attributes in the exported XML based on the values in the part's record
            var element = context.Element(part.PartDefinition.Name);
            if (part.Record.ParentTerritory != null) {
                element.SetAttributeValue("ParentTerritoryId", GetIdentity(part.Record.ParentTerritory.Id));
            }
            if (part.Record.Hierarchy != null) {
                element.SetAttributeValue("HierarchyId", GetIdentity(part.Record.Hierarchy.Id));
            }
            if (part.Record.TerritoryInternalRecord != null) {
                element.SetAttributeValue("TerritoryInternalRecordId", part.Record.TerritoryInternalRecord.Name);
            }
        }

        protected override void Importing(TerritoryPart part, ImportContentContext context) {
            // Set stuff in the record based off what is being imported
            if (context.Data.Element(part.PartDefinition.Name) == null) {
                return;
            }
            
            var hierarchyIdentity = context.Attribute(part.PartDefinition.Name, "HierarchyId");
            if (hierarchyIdentity == null) {
                part.Record.Hierarchy = null;
            } else {
                var ci = context.GetItemFromSession(hierarchyIdentity);
                var hierarchy = ci.As<TerritoryHierarchyPart>();
                _territoriesHierarchyService.AddTerritory(part, hierarchy);
            }
            
            var parentIdentity = context.Attribute(part.PartDefinition.Name, "ParentTerritoryId");
            if (parentIdentity == null) {
                part.Record.ParentTerritory = null;
            } else {
                var ci = context.GetItemFromSession(parentIdentity.ToString());
                var parent = ci.As<TerritoryPart>();
                _territoriesHierarchyService.AssignParent(part, parent);
            }


            var internalIdentity = context.Attribute(part.PartDefinition.Name, "TerritoryInternalRecordId");
            if (internalIdentity == null) {
                part.Record.TerritoryInternalRecord = null;
            } else {
                _territoriesHierarchyService.AssignInternalRecord(part, internalIdentity.ToString());
            }
        }

        private string GetIdentity(int id) {
            var ci = _contentManager.Get(id, VersionOptions.Latest);
            return _contentManager.GetItemMetadata(ci).Identity.ToString();
        }
    }
}
