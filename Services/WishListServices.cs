using Nwazet.Commerce.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Notify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.WishLists")]
    public class WishListServices : IWishListServices {
        private readonly IContentManager _contentManager;
        private readonly dynamic _shapeFactory;
        private readonly IEnumerable<IProductAttributesDriver> _attributesDrivers;
        private readonly INotifier _notifier;
        private readonly IEnumerable<IWishListExtensionProvider> _wishListExtensionProviders;

        public WishListServices(
            IContentManager contentManager,
            IShapeFactory shapeFactory,
            IEnumerable<IProductAttributesDriver> attributesDrivers,
            INotifier notifier,
            IEnumerable<IWishListExtensionProvider> wishListExtensionProviders) {

            _contentManager = contentManager;
            _shapeFactory = shapeFactory;
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
            if (max > 0) {
                wishLists.AddRange(wishListsQuery.Slice(max - 1)); //-1, because the default wishlist is there already
            } else {
                wishLists.AddRange(wishListsQuery.List());
            }

            return wishLists;
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
            if (wishlist.ContentItem.As<CommonPart>().Owner != user) {
                throw new ArgumentException(T("Users may only edit wishlists they own.").Text);
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
                //create a new wishlist element and add it
                var newELement = _contentManager.New<WishListElementPart>("WishListItem");
                newELement.WishListId = wishlist.ContentItem.Id;
                newELement.Item = item;
                _contentManager.Create(newELement.ContentItem);
                //add to list
                var elementIds = wishlist.Ids.ToList();
                elementIds.Add(newELement.ContentItem.Id);
                wishlist.Ids = elementIds.ToArray();

                //process extensions
                foreach (var ext in _wishListExtensionProviders) {
                    ext.WishListAddedElement(user, wishlist, newELement);
                }
            }
        }
        public IEnumerable<WishListElementPart> GetElements(IUser user, WishListListPart wishlist) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }
            if (wishlist.ContentItem.As<CommonPart>().Owner != user) {
                throw new ArgumentException(T("Users may only edit wishlists they own.").Text);
            }

            return GetElements(wishlist);
        }
        private IEnumerable<WishListElementPart> GetElements(WishListListPart wishlist) {
            return _contentManager.Query<WishListElementPart, WishListElementPartRecord>()
                .Where(epr => epr.WishListId == wishlist.ContentItem.Id)
                .List();
        }

        public bool ItemIsInWishlist(WishListListPart wishlist, ShoppingCartItem item) {
            return GetElements(wishlist).Any(wel =>
                ShoppingCartItem.ItemsAreEqual(wel.Item, item));
        }

        public void RemoveElementFromWishlist(IUser user, WishListListPart wishlist, int elementId) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }
            if (wishlist.ContentItem.As<CommonPart>().Owner != user) {
                throw new ArgumentException(T("Users may only edit wishlists they own.").Text);
            }

            var elementPart = _contentManager.Get<WishListElementPart>(elementId);
            
            RemoveElementFromWishlist(wishlist, elementPart);
        }
        private void RemoveElementFromWishlist(WishListListPart wishlist, WishListElementPart elementPart) {
            //process extensions
            foreach (var ext in _wishListExtensionProviders) {
                ext.WishListElementCleanup(wishlist, elementPart);
            }

            var elementId = elementPart.ContentItem.Id;
            if (wishlist.Ids.Contains(elementId)) {
                var elementIds = wishlist.Ids.ToList().Where(id => id != elementId);
                wishlist.Ids = elementIds.ToArray();
            }
            _contentManager.Destroy(elementPart.ContentItem); //hard delete
        }

        public void DeleteWishlist(IUser user, WishListListPart wishlist) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }
            if (wishlist.ContentItem.As<CommonPart>().Owner != user) {
                throw new ArgumentException(T("Users may only edit wishlists they own.").Text);
            }

            //remove all elements
            foreach (var element in GetElements(wishlist)) {
                RemoveElementFromWishlist(wishlist, element);
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

        public dynamic CreateShape(IUser user, ProductPart product = null) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            var productId = 0;
            var attributeShapes = new List<dynamic>();
            if (product != null) {
                productId = product.ContentItem.Id;
                attributeShapes = _attributesDrivers
                .Select(p => p.GetAttributeDisplayShape(product.ContentItem, _shapeFactory))
                .ToList();
            }
            //get the additional shapes from the extension providers
            var creationShapes = new List<dynamic>();
            //process extensions
            foreach (var ext in _wishListExtensionProviders) {
                creationShapes.Add(ext.BuildCreationShape(user, product));
            }

            return _shapeFactory.CreateNewWishList(
                ProductId: productId,
                AttributeShapes: attributeShapes,
                CreationShapes: creationShapes,
                WishListTitle: DefaultWishListTitle
                );
        }

        public dynamic SettingsShape(IUser user) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }
            //build the settings shape for each wishlist
            var settingsShapes = new List<dynamic>();
            var wishlists = GetWishLists(user);
            foreach (var ext in _wishListExtensionProviders) {
                settingsShapes.Add(ext.BuildSettingsShape(wishlists));
            }

            return _shapeFactory.WishListsSettings(
                WishLists: wishlists,
                SettingsShapes: settingsShapes
                );
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
