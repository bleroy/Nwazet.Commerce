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
        IEnumerable<WishListListPart> GetWishLists(IUser user, int max = 0);
        WishListListPart GetDefaultWishList(IUser user);
        WishListListPart GetWishList(IUser user, int wishListId = 0);
    }
}
