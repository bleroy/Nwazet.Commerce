using Orchard;
using Orchard.Security;

namespace Nwazet.Commerce.Services {
    public interface IWishListsPermissionsService : IDependency {
        void Adjust(CheckAccessContext context);
        void Checking(CheckAccessContext context);
        void Complete(CheckAccessContext context);
    }
}
