using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
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

        public WishListServices(
            IContentManager contentManager) {

            _contentManager = contentManager;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public IEnumerable<WishListListPart> GetWishLists(IUser user, int max = 0) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            var wishLists = new List<WishListListPart>();
            wishLists.Add(GetDefaultWishList(user));
            var wishListsQuery = _contentManager.Query<WishListListPart, WishListListPartRecord>()
               .Where(wllp => !wllp.IsDefault) //the default wishlist is already selected
               .Join<CommonPartRecord>()
               .Where(cpr => cpr.OwnerId == user.Id)
               .Join<TitlePartRecord>()
               .OrderBy(p => p.Title);
            if (max > 0) {
                wishLists.AddRange(wishListsQuery.Slice(max - 1));
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
                .Where(cpr => cpr.OwnerId == user.Id)
                .Slice(1).FirstOrDefault();
            if (wishList == null) {
                //create the wishlist if no default exists
                var ci = _contentManager.New("WishList");
                ci.As<WishListListPart>().IsDefault = true;
                ci.As<CommonPart>().Owner = user;
                ci.As<TitlePart>().Title = T("My WishList").Text;
                _contentManager.Create(ci);
                wishList = ci.As<WishListListPart>();
            }

            return wishList;
        }

        public WishListListPart GetWishList(IUser user, int wishListId = 0) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }
            if (wishListId <= 0) {
                return GetDefaultWishList(user);
            }
            var wishList = _contentManager.Query<WishListListPart, WishListListPartRecord>()
                .Where(pr => pr.ContentItemRecord.Id == wishListId)
                .Join<CommonPartRecord>()
                .Where(cpr => cpr.OwnerId == user.Id)
                .Slice(1).FirstOrDefault();

            if (wishList == null) {
                return GetDefaultWishList(user);
            }

            return wishList;
        }
    }
}
