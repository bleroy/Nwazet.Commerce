using Nwazet.Commerce.Models;
using Orchard;
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
    public class ProductPartWishListDriver : ContentPartDriver<ProductPart>{
        private readonly IWorkContextAccessor _wca;

        public ProductPartWishListDriver(
            IWorkContextAccessor wca) {

            _wca = wca;
        }

        protected override string Prefix {
            get { return "NwazetCommerceProduct"; }
        }

        protected override DriverResult Display(ProductPart part, string displayType, dynamic shapeHelper) {
            var user = _wca.GetContext().CurrentUser;
            //usually, wishlists only apply to authenticated users.
            //however, we are still going to show the "Add to list" functionalities, to drive sign-up / login
            var wishLists = new List<WishListListPart>();
            if (user != null) {
                //there is an authenticated user, who may have several lists
                //Get the wish lists for the user
                //A Site setting tells me how many lists I get at this stage.
                //we always take the default list

                //for each wishlist we found, build the shape we will display
                //as its "add to ..." link in the view
            }
            

            return ContentShape("Parts_Product_AddToWishlistButton",
                () => shapeHelper.Parts_Product_AddToWishlistButton(
                    ProductId: part.Id,
                    User: user,
                    WishLists: wishLists,
                    Prefix: Prefix));
        }
    }
}
