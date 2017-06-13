using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Orchard;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Nwazet.WishLists")]
    public class ProductPartWishListDriver : ContentPartDriver<ProductPart> {
        private readonly IWorkContextAccessor _wca;
        private readonly IWishListServices _wishListServices;
        private readonly IEnumerable<IProductAttributesDriver> _attributeDrivers;

        public ProductPartWishListDriver(
            IWorkContextAccessor wca,
            IWishListServices wishListServices,
            IEnumerable<IProductAttributesDriver> attributeDrivers) {

            _wca = wca;
            _wishListServices = wishListServices;
            _attributeDrivers = attributeDrivers;
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
                //we always take at least the default list
                wishLists = _wishListServices.GetWishLists(user).ToList();
                //for each wishlist we found, build the shape we will display
                //as its "add to ..." link in the view
            }

            // Get attributes and add them to the add to list shape
            var attributeShapes = _attributeDrivers
                .Select(p => p.GetAttributeDisplayShape(part.ContentItem, shapeHelper))
                .ToList();
            return ContentShape("Parts_Product_AddToWishlistButton", () => 
                shapeHelper.Parts_Product_AddToWishlistButton(
                    ProductId: part.Id,
                    User: user,
                    WishLists: wishLists,
                    Prefix: Prefix,
                    CreateShape: _wishListServices.CreateShape(user, part),
                    AttributeShapes: attributeShapes
                    ));
        }
    }
}
