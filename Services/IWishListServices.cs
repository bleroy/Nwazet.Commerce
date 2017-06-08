using Nwazet.Commerce.Models;
using Orchard;
using Orchard.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nwazet.Commerce.Services {
    public interface IWishListServices : IDependency {
        /// <summary>
        /// Get the wishlists owned by the specified user.
        /// </summary>
        /// <param name="user">The user whose wishlists we are trying to get.</param>
        /// <param name="max">Optionally, tell how many wishlists this call can return.</param>
        /// <returns>An IEnumerable<WishListListPart> of the requested wishlists.</returns>
        IEnumerable<WishListListPart> GetWishLists(IUser user, int max = 0);
        /// <summary>
        /// Gets the default wishlist for the specified user.
        /// If the user does not have a default wishlist yet, one is created.
        /// </summary>
        /// <param name="user">The user whose wishlists we are trying to get.</param>
        /// <returns>The user's default wishlist.</returns>
        WishListListPart GetDefaultWishList(IUser user);
        /// <summary>
        /// Gets a specific wishlist of the specified user.
        /// If the specific wishlist cannot be found or accessed, the default wishlist is 
        /// returned instead.
        /// </summary>
        /// <param name="user">The user whose wishlists we are trying to get.</param>
        /// <param name="wishListId">The id of the wishlist.</param>
        /// <returns>The specific wishlist.</returns>
        WishListListPart GetWishList(IUser user, int wishListId = 0);
        /// <summary>
        /// Creates a new wish list for the given user and with the given title
        /// </summary>
        /// <param name="user">The user for whom a wish list is being created.</param>
        /// <param name="title">The title of the new wish list</param>
        /// <returns>The newly created wish list</returns>
        WishListListPart CreateWishList(IUser user, string title = null);
        /// <summary>
        /// Adds the specific product, with its attributes, to the wish list.
        /// </summary>
        /// <param name="user">The user whose wishlist we are updating</param>
        /// <param name="wishlist">The wish list we are updating</param>
        /// <param name="product">The product to add</param>
        /// <param name="attributes">The product's attributes</param>
        void AddProductToWishList(IUser user, WishListListPart wishlist, ProductPart product, IDictionary<int, ProductAttributeValueExtended> attributes);
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
        /// <returns>A shape that can be displayed.</returns>
        dynamic SettingsShape(IUser user);
    }
}
