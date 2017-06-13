using Nwazet.Commerce.Models;
using Orchard;
using Orchard.Security;
using System.Collections.Generic;

namespace Nwazet.Commerce.Services {
    public interface IWishListExtensionProvider : IDependency {

        /// <summary>
        /// The name of the extension provider
        /// </summary>
        string Name { get; }
        /// <summary>
        /// The name displayed for the extension provider
        /// </summary>
        string DisplayName { get; }
        /// <summary>
        /// Provides a shape to be used during the dispaly of a wishlist's element
        /// </summary>
        /// <param name="elementPart">The the element to display</param>
        /// <returns>The shape</returns>
        dynamic BuildElementDisplayShape(WishListElementPart elementPart);
        /// <summary>
        /// Provides a shape to be used during the dispaly of a wishlist
        /// </summary>
        /// <param name="wishlist">The the wishlist to display</param>
        /// <returns>The shape</returns>
        dynamic BuildWishListDisplayShape(WishListListPart wishlist);
        /// <summary>
        /// Provides a shape to be used during the creation of a new wishlist
        /// </summary>
        /// <param name="user">The user for whom a wish list is being created.</param>
        /// <param name="product">The product we may wish to add to the new wishlist</param>
        /// <returns>The shape</returns>
        dynamic BuildCreationShape(IUser user, ProductPart productPart);
        /// <summary>
        /// Provides a shape with additional settings for wishlists.
        /// Note that these are not the site's settings, but a user's per-list settings.
        /// </summary>
        /// <param name="wishlists">The wish lists whose settings we are about to display</param>
        /// <returns>The shape</returns>
        dynamic BuildSettingsShape(IEnumerable<WishListListPart> wishlists);
        /// <summary>
        /// Perform cleanup operations when a wishlist is being deleted
        /// </summary>
        /// <param name="user">The user whose wishlist we are deleting</param>
        /// <param name="wishlist">The wish list we are deleting</param>
        void WishListCleanup(IUser user, WishListListPart wishlist);
        /// <summary>
        /// Perform cleanup operations when an element is being deleted
        /// </summary>
        /// <param name="wishlist">The wish list we are deleting</param>
        /// <param name="elementPart">The the element to remove</param>
        void WishListElementCleanup(WishListListPart wishlist, WishListElementPart elementPart);
        /// <summary>
        /// Perform additional operations when an wish list is being created
        /// </summary>
        /// <param name="user">The user whose wishlist we are creating</param>
        /// <param name="wishlist">The wish list we are creating</param>
        void WishListCreation(IUser user, WishListListPart wishlist);
        /// <summary>
        /// Perform additional operations when an element is being added to a list
        /// </summary>
        /// <param name="user">The user whose wishlist we are updating</param>
        /// <param name="wishlist">The wish list we are updating</param>
        /// <param name="elementPart">The the element we are adding</param>
        void WishListAddedElement(IUser user, WishListListPart wishlist, WishListElementPart elementPart);
        /// <summary>
        /// Perform the extension's operations related to wishlist settings
        /// </summary>
        /// <param name="user">The user whose wishlist we are updating</param>
        void UpdateSettings(IUser user, WishListListPart wishlist);
    }
}
