using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Recipes.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Nwazet.Commerce.Recipes.Providers.Builders {
    [OrchardFeature("Territories")]
    public class TerritoriesStep : RecipeBuilderStep {

        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IContentManager _contentManager;
        private readonly IContentDefinitionWriter _contentDefinitionWriter;
        private readonly ITerritoriesRepositoryService _territoriesRepositoryService;

        public TerritoriesStep(
            IContentDefinitionManager contentDefinitionManager,
            IContentManager contentManager,
            IContentDefinitionWriter contentDefinitionWriter,
            ITerritoriesRepositoryService territoriesRepositoryService) {

            _contentDefinitionManager = contentDefinitionManager;
            _contentManager = contentManager;
            _contentDefinitionWriter = contentDefinitionWriter;
            _territoriesRepositoryService = territoriesRepositoryService;
        }

        public override LocalizedString Description {
            get { return T("Exports definitions, items and identity records. This is the recommended way to export territories and hierarchies."); }
        }

        public override LocalizedString DisplayName {
            get { return T("Territories and Hierarchies"); }
        }

        public override string Name {
            get { return "Territories"; }
        }

        // Priority is used to order steps during execution. Steps at higher priority are executed first.
        public override int Priority { get { return 20; } }
        // Position is used to order steps when displaying them in the UI. We set it lower than
        // what is defined for the ContentStep, so that this step is shown before.
        public override int Position { get { return 15; } }
        
        public override void Build(BuildContext context) {
            // Get all ContentTypes that contain TerritoryHierarchyPart or TerritoryPart
            // because we will export their definitions and all ContentItems.
            var hierarchyTypes = _contentDefinitionManager.ListTypeDefinitions()
                .Where(ctd => ctd.Parts.Any(pa => pa
                    .PartDefinition.Name.Equals(TerritoryHierarchyPart.PartName, StringComparison.InvariantCultureIgnoreCase)))
                .Select(ctd => ctd.Name);
            var territoryTypes = _contentDefinitionManager.ListTypeDefinitions()
                .Where(ctd => ctd.Parts.Any(pa => pa
                    .PartDefinition.Name.Equals(TerritoryPart.PartName, StringComparison.InvariantCultureIgnoreCase)))
                .Select(ctd => ctd.Name);

            // Add a command telling to try and enable this feature
            context.RecipeDocument.Element("Orchard")
                .Add(EnableFeatureElement());

            // Export the TerritoryInternalRecords: This adds to the exported xml commands to recreate the records.
            context.RecipeDocument.Element("Orchard")
                .Add(ExportInternalRecordsCommands());

            // Export ContentDefinitions
            if (hierarchyTypes.Any())
                context.RecipeDocument.Element("Orchard")
                    .Add(ExportMetadata(hierarchyTypes));
            if (territoryTypes.Any())
                context.RecipeDocument.Element("Orchard")
                    .Add(ExportMetadata(territoryTypes));

            // Export ContentItems
            // 1. Export Hierarchies
            // Get the ContentItems for the hierarchies (they have references to their territories)
            var hierarchyItems = hierarchyTypes.Any()
                ? _contentManager
                    .Query(VersionOptions.Latest, hierarchyTypes.ToArray())
                    .List().ToList()
                : Enumerable.Empty<ContentItem>().ToList();
            if (hierarchyItems.Any())
                context.RecipeDocument.Element("Orchard")
                    .Add(ExportData(hierarchyTypes, hierarchyItems));
            // 2. Export Territories
            foreach (var hierarchy in hierarchyItems.Select(ci => ci.As<TerritoryHierarchyPart>())) {
                if (hierarchy.Territories.Any()) {
                    context.RecipeDocument.Element("Orchard")
                    .Add(ExportTerritories(hierarchy.Territories));
                }
            }
        }

        /// <summary>
        /// from Orchard.Recipes.Providers.Builders.ContentStep. Hierarchies can be exported normally,
        /// as long as they are exported/imported before territories.
        /// </summary>
        /// <param name="contentTypes"></param>
        /// <returns></returns>
        private XElement ExportMetadata(IEnumerable<string> contentTypes) {
            var typesElement = new XElement("Types");
            var partsElement = new XElement("Parts");
            var typesToExport = _contentDefinitionManager.ListTypeDefinitions()
                .Where(typeDefinition => contentTypes.Contains(typeDefinition.Name))
                .ToList();
            var partsToExport = new Dictionary<string, ContentPartDefinition>();

            foreach (var contentTypeDefinition in typesToExport.OrderBy(x => x.Name)) {
                foreach (var contentPartDefinition in contentTypeDefinition.Parts.OrderBy(x => x.PartDefinition.Name)) {
                    if (partsToExport.ContainsKey(contentPartDefinition.PartDefinition.Name)) {
                        continue;
                    }
                    partsToExport.Add(contentPartDefinition.PartDefinition.Name, contentPartDefinition.PartDefinition);
                }
                typesElement.Add(_contentDefinitionWriter.Export(contentTypeDefinition));
            }

            foreach (var part in partsToExport.Values.OrderBy(x => x.Name)) {
                partsElement.Add(_contentDefinitionWriter.Export(part));
            }

            return new XElement("ContentDefinition", typesElement, partsElement);
        }

        /// <summary>
        /// from Orchard.Recipes.Providers.Builders.ContentStep
        /// </summary>
        /// <param name="contentTypes"></param>
        /// <param name="contentItems"></param>
        /// <returns></returns>
        private XElement ExportData(IEnumerable<string> contentTypes, IEnumerable<ContentItem> contentItems) {
            var data = new XElement("Content");

            var orderedContentItemsQuery =
                from contentItem in contentItems
                let identity = _contentManager.GetItemMetadata(contentItem).Identity.ToString()
                orderby identity
                select contentItem;

            var orderedContentItems = orderedContentItemsQuery.ToList();

            foreach (var contentType in contentTypes.OrderBy(x => x)) {
                var type = contentType;
                var items = orderedContentItems.Where(i => i.ContentType == type);
                foreach (var contentItem in items) {
                    var contentItemElement = _contentManager.Export(contentItem);
                    if (contentItemElement != null)
                        data.Add(contentItemElement);
                }
            }

            return data;
        }

        private XElement ExportTerritories(IEnumerable<ContentItem> contentItems) {
            var data = new XElement("Content");
            
            // Group territories by hierarchy
            var groups = contentItems
                .GroupBy(ci => ci.As<TerritoryPart>().Record.Hierarchy.Id);

            // If we don't export stuff in the right order, import may mess up
            foreach (var hierarchyGroup in groups) {
                var exported = new List<int>();
                // Export first level
                foreach (var item in hierarchyGroup.Where(ci => ci.As<TerritoryPart>().Record.ParentTerritory == null)) {
                    var element = _contentManager.Export(item);
                    if (element != null) {
                        data.Add(element);
                    }
                    exported.Add(item.As<TerritoryPart>().Record.Id);
                }
                while (exported.Count < hierarchyGroup.Count()) {
                    foreach (var item in hierarchyGroup
                        .Where(ci => 
                            ci.As<TerritoryPart>().Record.ParentTerritory != null
                            && !exported.Contains(ci.As<TerritoryPart>().Record.Id)
                            && exported.Contains(ci.As<TerritoryPart>().Record.ParentTerritory.Id))) {
                        var element = _contentManager.Export(item);
                        if (element != null) {
                            data.Add(element);
                        }
                        exported.Add(item.As<TerritoryPart>().Record.Id);
                    }
                }
            }

            return data;
        }

        private XElement EnableFeatureElement() {
            var root = new XElement("Feature");
            root.Add(new XAttribute("enable", "Territories"));
            return root;
        }

        private XElement ExportInternalRecordsCommands() {
            var root = new XElement("Command");
            root.Add(Environment.NewLine);
            foreach (var tir in _territoriesRepositoryService.GetTerritories()) {
                root.Add($"territories import \"{tir.Name}\"" + Environment.NewLine);
            }
            return root;
        }
    }
}
