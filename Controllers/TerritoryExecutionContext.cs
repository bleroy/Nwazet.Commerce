using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Nwazet.Commerce.Controllers {
    [OrchardFeature("Territories")]
    public class TerritoryExecutionContext {
        public ContentItem HierarchyItem { get; set; }
        public ContentItem TerritoryItem { get; set; }
        public LocalizedString Message { get; set; }
        public IEnumerable<Permission> AdditionalPermissions { get; set; }
        public Func<ContentItem, ActionResult> ExecutionAction { get; set; }
        
        public TerritoryExecutionContext() {
            AdditionalPermissions = Enumerable.Empty<Permission>();
        }
    }
}
