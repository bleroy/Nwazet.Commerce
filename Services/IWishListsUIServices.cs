using Nwazet.Commerce.Models;
using Orchard;
using Orchard.Security;

namespace Nwazet.Commerce.Services {
    public interface IWishListsUIServices : IDependency {
        /// <summary>
        /// Generates the shape for the creation of a new wishlist.
        /// </summary>
        /// <param name="user">The user whose wishlists we are trying to get.</param>
        /// <param name="product">The product we may wish to add to the new wishlist</param>
        /// <returns>A shape that can be displayed.</returns>
        dynamic CreateShape(IUser user, ProductPart product = null);
        /// <summary>
        /// Generate the shape to access the settings of a users wishlists
        /// </summary>
        /// <param name="user">The user whose wishlists we are trying to get.</param>
        /// <param name="wishListId">The id of the wish list we are currently displaying.</param>
        /// <returns>A shape that can be displayed.</returns>
        dynamic SettingsShape(IUser user, int wishListId = 0);
    }
}
