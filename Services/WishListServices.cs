using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Security;
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
        private readonly IEnumerable<IProductAttributesDriver> _attributeDrivers;

        public WishListServices(
            IContentManager contentManager,
            IShapeFactory shapeFactory,
            IEnumerable<IProductAttributesDriver> attributeDrivers) {

            _contentManager = contentManager;
            _shapeFactory = shapeFactory;
            _attributeDrivers = attributeDrivers;

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

            return ci.As<WishListListPart>();
        }

        public void AddProductToWishList(IUser user, WishListListPart wishlist, ProductPart product, Dictionary<int, ProductAttributeValueExtended> attributes) {

        }

        public dynamic CreateShape(IUser user, ProductPart product = null) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            var productId = 0;
            var attributeShapes = new List<dynamic>();
            if (product!=null) {
                productId = product.ContentItem.Id;
                attributeShapes = _attributeDrivers
                .Select(p => p.GetAttributeDisplayShape(product.ContentItem, _shapeFactory))
                .ToList();
            }
            //get the additional shapes from the extension providers
            var creationShapes = new List<dynamic>();

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
            return _shapeFactory.WishListsSettings();
        }
    }
}
