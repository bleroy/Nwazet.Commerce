using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nwazet.Commerce.ViewModels {
    public class HierarchyIndexEntry {

        public int Id { get; set; }

        public string DisplayText { get; set; }

        public ContentItem ContentItem { get; set; }

        public string TypeDisplayName { get; set; }

        public bool IsDraft { get; set; }

        public int TerritoriesCount { get; set; }

        public string TerritoryTypeDisplayName { get; set; }
    }
}
