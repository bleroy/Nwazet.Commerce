using Orchard.ContentManagement.MetaData.Models;
using Orchard.Environment.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nwazet.Commerce.ViewModels {
    [OrchardFeature("Territories")]
    public class HierarchyAdminIndexViewModel {

        public IList<ContentTypeDefinition> AllowedHierarchyTypes { get; set; }

        public IList<HierarchyIndexEntry> HierarchyEntries { get; set; }

        public dynamic Pager { get; set; }
    }
}
