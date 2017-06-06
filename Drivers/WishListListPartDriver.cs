using Nwazet.Commerce.Models;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Nwazet.WishLists")]
    public class WishListListPartDriver : ContentPartDriver<WishListListPart> {

        public WishListListPartDriver() { }

        protected override string Prefix {
            get { return "NwazetCommerceWishList"; }
        }

        protected override DriverResult Display(WishListListPart part, string displayType, dynamic shapeHelper) {
            return null;
        }
    }
}
