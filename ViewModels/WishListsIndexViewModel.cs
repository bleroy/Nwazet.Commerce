using Nwazet.Commerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nwazet.Commerce.ViewModels {
    public class WishListsIndexViewModel {
        public IEnumerable<WishListListPart> WishLists { get; set; }
        public WishListListPart WishList { get; set; }
    }
}
