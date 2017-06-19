using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Environment.Extensions;
using Orchard.Security;

namespace Nwazet.Commerce.Security {
    [OrchardFeature("Nwazet.WishLists")]
    public class WishListsAuthorizationEvenHandler : IAuthorizationServiceEventHandler {
        public void Adjust(CheckAccessContext context) { }

        public void Checking(CheckAccessContext context) { }

        public void Complete(CheckAccessContext context) {
            /*
             context.Permission: the permission we are checking
             context.User: The request's current user
             context.Content: the content item we are trying to access
             context.Granted: set to true if we grant permission; false otherwise
             context.Adjusted: set to true if we changed the value of Granted
             */
            var wishList = context.Content.As<WishListListPart>();
            if (wishList != null) {
                var owner = context.Content.As<CommonPart>().Owner;
                var oldGranted = context.Granted;
                context.Granted &= owner == context.User; //wish lists are always private
                context.Adjusted = context.Granted != oldGranted;
            }
        }
    }
}
