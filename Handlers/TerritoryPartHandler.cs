using Nwazet.Commerce.Extensions;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;

namespace Nwazet.Commerce.Handlers {
    [OrchardFeature("Territories")]
    public class TerritoryPartHandler : ContentHandler {

        private readonly IContentManager _contentManager;

        public TerritoryPartHandler(
            IRepository<TerritoryPartRecord> repository,
            IContentManager contentManager) {

            _contentManager = contentManager;

            Filters.Add(StorageFilter.For(repository));

            ////Lazyfield setters
            OnInitializing<TerritoryPart>(PropertySetHandlers);
            OnLoading<TerritoryPart>((context, part) => LazyLoadHandlers(part));
            OnVersioning<TerritoryPart>((context, part, newVersionPart) => LazyLoadHandlers(newVersionPart));

            //Handle the presence of child territories: may need to run asynchronously
            OnRemoving<TerritoryPart>(RemoveChildren);
            // Clean the record up, to avoid issues with the 1-to-many relationships
            OnRemoving<TerritoryPart>(CleanupRecord);
        }

        protected override void GetItemMetadata(GetContentItemMetadataContext context) {
            var territory = context.ContentItem.As<TerritoryPart>();

            if (territory == null)
                return;

            context.Metadata.EditorRouteValues = new RouteValueDictionary {
                {"Area", TerritoriesUtilities.Area},
                {"Controller", TerritoriesUtilities.TerritoryEditController},
                {"Action", TerritoriesUtilities.TerritoryEditAction},
                {"Id", context.ContentItem.Id}
            };
        }

        static void PropertySetHandlers(
            InitializingContentContext context, TerritoryPart part) {

            part.ChildrenField.Setter(value => {
                var actualItems = value
                    .Where(ci => ci.As<TerritoryPart>() != null);
                part.Record.Children = actualItems.Any() ?
                    actualItems.Select(ci => ci.As<TerritoryPart>().Record).ToList() :
                    new List<TerritoryPartRecord>();
                return actualItems;
            });

            part.HierarchyField.Setter(hierarchy => {
                part.Record.Hierarchy = hierarchy.As<TerritoryHierarchyPart>().Record;
                return hierarchy;
            });

            part.ParentField.Setter(parent => {
                part.Record.ParentTerritory = parent.As<TerritoryPart>().Record;
                return parent;
            });

            //call the setters in case a value had already been set
            if (part.ChildrenField.Value != null) {
                part.ChildrenField.Value = part.ChildrenField.Value;
            }
            if (part.HierarchyField.Value != null) {
                part.HierarchyField.Value = part.HierarchyField.Value;
            }
            if (part.ParentField.Value != null) {
                part.ParentField.Value = part.ParentField.Value;
            }
        }

        void LazyLoadHandlers(TerritoryPart part) {

            part.ChildrenField.Loader(() => {
                if (part.Record.Children != null && part.Record.Children.Any()) {
                    return _contentManager
                        .GetMany<ContentItem>(part.Record.Children.Select(tpr => tpr.ContentItemRecord.Id),
                            VersionOptions.Latest, QueryHints.Empty);
                } else {
                    return Enumerable.Empty<ContentItem>();
                }
            });

            part.HierarchyField.Loader(() => {
                if (part.Record.Hierarchy != null) {
                    return _contentManager
                        .Get<ContentItem>(part.Record.Hierarchy.Id,
                            VersionOptions.Latest, QueryHints.Empty);
                } else {
                    return null;
                }

            });

            part.ParentField.Loader(() => {
                if (part.Record.ParentTerritory != null) {
                    return _contentManager
                        .Get<ContentItem>(part.Record.ParentTerritory.Id,
                            VersionOptions.Latest, QueryHints.Empty);
                } else {
                    return null;
                }

            });

            part.AllChildrenCountField.Loader(() => {
                if (part.Record != null) {
                    return CountChildren(part.Record);
                }
                return 0;
            });
        }

        private int CountChildren(TerritoryPartRecord tpr) {
            if (tpr.Children == null || !tpr.Children.Any()) {
                return 0;
            }
            return tpr.Children.Count + tpr.Children.Sum(CountChildren);
        }

        void RemoveChildren(RemoveContentContext context, TerritoryPart part) {
            // Only remove first level of children, because they will remove their children
            foreach (var item in part.Children) {
                _contentManager.Remove(item);
            }
        }

        void CleanupRecord(RemoveContentContext context, TerritoryPart part) {
            part.Record.Hierarchy = null;
            part.Record.ParentTerritory = null;
            part.Record.TerritoryInternalRecord = null;
        }
    }
}
