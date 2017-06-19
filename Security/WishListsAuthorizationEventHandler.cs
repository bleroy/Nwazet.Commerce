using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Security;

namespace Nwazet.Commerce.Security {
    [OrchardFeature("Nwazet.WishLists")]
    public class WishListsAuthorizationEventHandler : IAuthorizationServiceEventHandler {
        private readonly IWishListsPermissionsService _wishListsPermissionsService;

        public WishListsAuthorizationEventHandler(
            IWishListsPermissionsService wishListsPermissionsService) {
            //the reason we use a service rather than doing things directly here is to
            //give the ability to override the default behaviour. Event handlers such as
            //this class are not automatically overridden when a new one is instantiated,
            //while only the service of the correct type with the highest priority is
            //injected here.

            _wishListsPermissionsService = wishListsPermissionsService;
        }
        public void Adjust(CheckAccessContext context) {
            if (context.Content.As<WishListListPart>() != null) {
                _wishListsPermissionsService.Adjust(context);
            }
        }

        public void Checking(CheckAccessContext context) {
            if (context.Content.As<WishListListPart>() != null) {
                _wishListsPermissionsService.Checking(context);
            }
        }

        public void Complete(CheckAccessContext context) {
            if (context.Content.As<WishListListPart>() != null) {
                _wishListsPermissionsService.Complete(context);
            }
        }
    }
}
