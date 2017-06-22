using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Notify;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.WishLists")]
    public class WishListServices : IWishListServices {
        private readonly IContentManager _contentManager;
        private readonly IEnumerable<IProductAttributesDriver> _attributesDrivers;
        private readonly INotifier _notifier;
        private readonly IEnumerable<IWishListExtensionProvider> _wishListExtensionProviders;

        public WishListServices(
            IContentManager contentManager,
            IEnumerable<IProductAttributesDriver> attributesDrivers,
            INotifier notifier,
            IEnumerable<IWishListExtensionProvider> wishListExtensionProviders) {

            _contentManager = contentManager;
            _attributesDrivers = attributesDrivers;
            _notifier = notifier;
            _wishListExtensionProviders = wishListExtensionProviders;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        private string DefaultWishListTitle {
            get { return T("My WishList").Text; }
        }

        public IEnumerable<WishListListPart> GetWishLists(IUser user, int max = 0) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            var wishLists = new List<WishListListPart>();
            wishLists.Add(GetDefaultWishList(user));
            var wishListsQuery = _contentManager.Query<WishListListPart, WishListListPartRecord>()
               .Where(wllp => !wllp.IsDefault) //the default wishlist is already selected
               .Join<CommonPartRecord>()
               .Where(cpr => cpr.OwnerId == user.Id) //must match owner
               .Join<TitlePartRecord>()
               .OrderBy(p => p.Title);
            
            return max > 0 ?
                wishLists.Concat(wishListsQuery.Slice(max - 1)) : //-1, because the default wishlist is there already
                wishLists.Concat(wishListsQuery.List());
        }

        public WishListListPart GetDefaultWishList(IUser user) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }
            //get default wishlist
            var wishList = _contentManager.Query<WishListListPart, WishListListPartRecord>()
                .Where(wllp => wllp.IsDefault)
                .Join<CommonPartRecord>()
                .Where(cpr => cpr.OwnerId == user.Id) //match owner
                .Slice(1).FirstOrDefault();
            if (wishList == null) {
                //if there are wishlists, make the first one default
                wishList = _contentManager.Query<WishListListPart, WishListListPartRecord>()
                    .Join<CommonPartRecord>()
                    .Where(cpr => cpr.OwnerId == user.Id)
                    .Slice(1).FirstOrDefault();
                if (wishList == null) { //no wishlist exists
                    //create the wishlist if no default one exists
                    var ci = _contentManager.New("WishList");
                    ci.As<WishListListPart>().IsDefault = true;
                    ci.As<CommonPart>().Owner = user;
                    ci.As<TitlePart>().Title = DefaultWishListTitle;
                    _contentManager.Create(ci);
                    wishList = ci.As<WishListListPart>();
                    //process extensions
                    foreach (var ext in _wishListExtensionProviders) {
                        ext.WishListCreation(user, wishList);
                    }
                } else {
                    wishList.IsDefault = true;
                }
            }

            return wishList;
        }

        public WishListListPart GetWishList(IUser user, int wishListId = 0) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }
            if (wishListId <= 0) { //invalid Id
                return GetDefaultWishList(user);
            }
            var wishList = _contentManager.Query<WishListListPart, WishListListPartRecord>()
                .Where(pr => pr.ContentItemRecord.Id == wishListId)
                .Join<CommonPartRecord>()
                .Where(cpr => cpr.OwnerId == user.Id) //match owner
                .Slice(1).FirstOrDefault();

            if (wishList == null) { //wishlist does not exist or does not belong to user
                return GetDefaultWishList(user);
            }

            return wishList;
        }

        private WishListListPart GetWishList(int wishListId) {
            return _contentManager.Query<WishListListPart, WishListListPartRecord>()
               .Where(pr => pr.ContentItemRecord.Id == wishListId)
               .Slice(1).FirstOrDefault();
        }

        public bool TryGetWishList(IUser user, out WishListListPart wishList, int wishListId = 0) {
            var ret = true;
            if (wishListId == 0) {
                wishList = GetDefaultWishList(user);
            } else {
                wishList = GetWishList(user, wishListId);
                ret = wishListId == wishList.ContentItem.Id;
            }
            return ret;
        }

        public bool TryGetWishList(out WishListListPart wishList, int wishListId = 0) {
            wishList = GetWishList(wishListId);
            return wishList != null;
        }

        public WishListListPart CreateWishList(IUser user, string title = null) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }
            if (string.IsNullOrWhiteSpace(title)) {
                title = DefaultWishListTitle;
            }

            var ci = _contentManager.New("WishList");
            ci.As<WishListListPart>().IsDefault = false;
            ci.As<CommonPart>().Owner = user;
            ci.As<TitlePart>().Title = title;
            _contentManager.Create(ci);

            //process extensions
            foreach (var ext in _wishListExtensionProviders) {
                ext.WishListCreation(user, ci.As<WishListListPart>());
            }

            return ci.As<WishListListPart>();
        }

        public void AddProductToWishList(IUser user, WishListListPart wishlist, ProductPart product, IDictionary<int, ProductAttributeValueExtended> attributes) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }
            //we can add the product to the wishlist
            if (!ValidateAttributes(product.ContentItem.Id, attributes)) {
                // If attributes don't validate, don't add the product, but notify
                _notifier.Warning(T("Couldn't add this product because of invalid attributes. Please refresh the page and try again."));
                return;
            }
            //compute the ShoppingCartItem for the product we are adding
            var item = new ShoppingCartItem(product.ContentItem.Id, 1, attributes);
            //check whether the product is in the wishlist already
            if (!ItemIsInWishlist(wishlist, item)) {
                //create a new wishlist item and add it
                var newItem = _contentManager.New<WishListItemPart>("WishListItem");
                newItem.WishListId = wishlist.ContentItem.Id;
                newItem.Item = item;
                _contentManager.Create(newItem.ContentItem);

                //process extensions
                foreach (var ext in _wishListExtensionProviders) {
                    ext.WishListAddedItem(user, wishlist, newItem);
                }
            }
        }
        
        private IEnumerable<WishListItemPart> GetItems(WishListListPart wishlist) {
            return wishlist.WishListItems.Select(it => it.As<WishListItemPart>());
        }

        public bool ItemIsInWishlist(WishListListPart wishlist, ShoppingCartItem item) {
            return GetItems(wishlist).Any(wel =>
                ShoppingCartItem.ItemsAreEqual(wel.Item, item));
        }

        public void RemoveItemFromWishlist(WishListListPart wishlist, int itemId) {
            RemoveItemFromWishlist(wishlist, _contentManager.Get<WishListItemPart>(itemId));
        }
        private void RemoveItemFromWishlist(WishListListPart wishlist, WishListItemPart itemPart) {
            //process extensions
            foreach (var ext in _wishListExtensionProviders) {
                ext.WishListItemCleanup(wishlist, itemPart);
            }

            var elementId = itemPart.ContentItem.Id;
            _contentManager.Destroy(itemPart.ContentItem); //hard delete
        }

        public void DeleteWishlist(WishListListPart wishlist) {
            var user = wishlist.ContentItem.As<CommonPart>().Owner;
            //remove all elements
            foreach (var element in GetItems(wishlist)) {
                RemoveItemFromWishlist(wishlist, element);
            }
            //destroy wishlist
            //process extensions
            foreach (var ext in _wishListExtensionProviders) {
                ext.WishListCleanup(user, wishlist);
            }
            if (wishlist.IsDefault) {//user must still have a default
                var otherLists = GetWishLists(user).Where(wl => wl.ContentItem.Id != wishlist.ContentItem.Id);
                if (otherLists.Any()) {
                    otherLists.First().IsDefault = true;
                } else {
                    GetDefaultWishList(user); //create a new default wishlist
                }
                wishlist.IsDefault = false;
            }
            _contentManager.Destroy(wishlist.ContentItem);
        }
        
        private bool ValidateAttributes(int productId, IDictionary<int, ProductAttributeValueExtended> attributeIdsToValues) {
            if (_attributesDrivers == null ||
                attributeIdsToValues == null ||
                !attributeIdsToValues.Any()) return true;

            var product = _contentManager.Get(productId);
            return _attributesDrivers.All(d => d.ValidateAttributes(product, attributeIdsToValues));
        }
        
    }
}
