using Nwazet.Commerce.Models;
using Nwazet.Commerce.Permissions;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Security;
using System.Linq;

namespace Nwazet.Commerce.Security {
    [OrchardFeature("Territories")]
    public class TerritoriesAuthorizationEventHandler : IAuthorizationServiceEventHandler {
        public void Adjust(CheckAccessContext context) {
            if (!context.Granted &&
                context.Content.Is<TerritoryPart>()) {

                var typeDefinition = context.Content.ContentItem.TypeDefinition;
                //replace permission if there is one specific for the content type
                if (typeDefinition.Parts.Any(ctpd => ctpd.PartDefinition.Name == TerritoryPart.PartName) &&
                    context.Permission == TerritoriesPermissions.ManageTerritories) {
                    context.Adjusted = true;
                    context.Permission = TerritoriesPermissions.GetTerritoryPermission(typeDefinition);
                }
            }
        }

        #region not implemented IAuthorizationServiceEventHandler methods
        public void Checking(CheckAccessContext context) { }

        public void Complete(CheckAccessContext context) { }
        #endregion
    }
}
