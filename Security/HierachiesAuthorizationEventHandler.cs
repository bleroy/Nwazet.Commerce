using Nwazet.Commerce.Models;
using Nwazet.Commerce.Permissions;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nwazet.Commerce.Security {
    [OrchardFeature("Territories")]
    public class HierachiesAuthorizationEventHandler : IAuthorizationServiceEventHandler {
        public void Adjust(CheckAccessContext context) {
            if (!context.Granted &&
                context.Content.Is<TerritoryHierarchyPart>()) {

                var typeDefinition = context.Content.ContentItem.TypeDefinition;
                //replace permission if there is one specific for the content type
                if (typeDefinition.Parts.Any(ctpd => ctpd.PartDefinition.Name == TerritoryHierarchyPart.PartName) &&
                    context.Permission == TerritoriesPermissions.ManageTerritoryHierarchies) {
                    context.Adjusted = true;
                    context.Permission = TerritoriesPermissions.GetHierarchyPermission(typeDefinition);
                }
            }
        }

        #region not implemented IAuthorizationServiceEventHandler methods
        public void Checking(CheckAccessContext context) { }

        public void Complete(CheckAccessContext context) { }
        #endregion
    }
}
