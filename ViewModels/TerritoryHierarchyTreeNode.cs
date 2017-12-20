using Orchard.ContentManagement;
using System.Collections.Generic;

namespace Nwazet.Commerce.ViewModels {
    public class TerritoryHierarchyTreeNode {
        // Items of this class will be serialized and used for the representation
        // of the territories in a hierarchy.

        // id of corresponding Territory ContentItem
        public int Id { get; set; }

        public ContentItem TerritoryItem { get; set; }

        public int ParentId { get; set; }

        // Url for the Edit page of the corresponding Territory ContentItem
        public string EditUrl { get; set; }

        // The "title" shown in the hierarchy for this territory
        public string DisplayText { get; set; }

        // Child-nodes of this node.
        // If this property is null it means we have not attempted to load its children.
        // This is different than the case where Nodes.Count() == 0, as that means we have
        // loaded the list of children, and it is empty.
        public IList<TerritoryHierarchyTreeNode> Nodes { get; set; }
        
        public TerritoryHierarchyTreeNode() {
        }
    }
}
