using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;
using Orchard.Environment.Extensions;
using System.Collections.Generic;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Territories")]
    public class TerritoryPart : ContentPart<TerritoryPartRecord> {

        public static string PartName = "TerritoryPart";

        private readonly LazyField<IEnumerable<ContentItem>> _children =
            new LazyField<IEnumerable<ContentItem>>();

        public LazyField<IEnumerable<ContentItem>> ChildrenField {
            get { return _children; }
        }

        // This contains the direct children of this territory
        public IEnumerable<ContentItem> Children {
            get { return _children.Value; }
            // no setter, because this is "filled" thanks to a 1-to-n relationship to TerritoryPartRecords
        }

        private readonly LazyField<ContentItem> _hierarchy =
            new LazyField<ContentItem>();

        public LazyField<ContentItem> HierarchyField {
            get { return _hierarchy; }
        }

        public ContentItem Hierarchy {
            get { return _hierarchy.Value; }
        }

        public TerritoryHierarchyPart HierarchyPart {
            get { return Hierarchy?.As<TerritoryHierarchyPart>(); }
        }

        private readonly LazyField<ContentItem> _parent =
            new LazyField<ContentItem>();

        public LazyField<ContentItem> ParentField {
            get { return _parent; }
        }

        public ContentItem Parent {
            get { return _parent.Value; }
        }

        public TerritoryPart ParentPart {
            get { return Parent?.As<TerritoryPart>(); }
        }

        private readonly LazyField<int> _allChildrenCount =
            new LazyField<int>();

        public LazyField<int> AllChildrenCountField {
            get { return _allChildrenCount; }
        }

        public int AllChildrenCount {
            get { return _allChildrenCount.Value; }
        }

        /// <summary>
        /// Verifies whether the current TerritoryPart matches the one passsed as a parameter by comparing the respective
        /// TerritoryInternalRecords. In case those cannot be accessed, or are null, the TerritoryParts are considered to 
        /// not match.
        /// </summary>
        /// <param name="other">The TerritoryPart whose TerritoryInternalRecord will be compared with the current one.</param>
        /// <returns>True if both TerritoryParts have a valid TerritoryInternalRecord, and the Ids of those match. False in 
        /// every other case.</returns>
        public bool IsSameAs(TerritoryPart other) {
            if (this.Record != null && this.Record.TerritoryInternalRecord != null
                && other.Record != null && other.Record.TerritoryInternalRecord != null
                && this.Record.TerritoryInternalRecord.Id == other.Record.TerritoryInternalRecord.Id) {

                return true;
            }
            return false;
        }
    }
}
