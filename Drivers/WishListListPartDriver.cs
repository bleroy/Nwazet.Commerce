using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Core.Common.Models;
using Orchard.Environment.Extensions;
using System.Collections.Generic;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Nwazet.WishLists")]
    public class WishListListPartDriver : ContentPartDriver<WishListListPart> {
        private readonly IContentManager _contentManager;
        private readonly IEnumerable<IWishListExtensionProvider> _wishListExtensionProviders;
        private readonly IWishListServices _wishListServices;
        private readonly IWishListsUIServices _wishListsUIServices;

        public WishListListPartDriver(
            IContentManager contentManager,
            IEnumerable<IWishListExtensionProvider> wishListExtensionProviders,
            IWishListServices wishListServices,
            IWishListsUIServices wishListsUIServices) {

            _contentManager = contentManager;
            _wishListExtensionProviders = wishListExtensionProviders;
            _wishListServices = wishListServices;
            _wishListsUIServices = wishListsUIServices;
        }

        protected override string Prefix {
            get { return "NwazetCommerceWishList"; }
        }

        protected override DriverResult Display(WishListListPart part, string displayType, dynamic shapeHelper) {
            var shapes = new List<DriverResult>(3);
            var user = part.ContentItem.As<CommonPart>().Owner;
            //get the elements out of the wishlist
            List<dynamic> elementsShapes = new List<dynamic>();

            foreach (var wlItem in part.WishListItems) {
                var itemPart = wlItem.As<WishListItemPart>();
                if (itemPart != null) {
                    elementsShapes.Add(_contentManager.BuildDisplay(itemPart));
                }
            }
            //Get the additional shapes form the extensions
            List<dynamic> extensionsShapes = new List<dynamic>();
            foreach (var ext in _wishListExtensionProviders) {
                extensionsShapes.Add(ext.BuildWishListDisplayShape(part));
            }

            shapes.Add(ContentShape("Parts_WishListList", () =>
               shapeHelper.Parts_WishListList(new WishListListViewModel {
                   ElementsShapes = elementsShapes,
                   ExtensionShapes = extensionsShapes
               })));
            shapes.Add(ContentShape("Parts_ListOfWishLists", () =>
                shapeHelper.Parts_ListOfWishLists(
                    WishLists: _wishListServices.GetWishLists(user)
                    )));
            shapes.Add(ContentShape("Parts_WishListsActions", () =>
                shapeHelper.Parts_WishListsActions(
                    CreateShape: _wishListsUIServices.CreateShape(user),
                    SettingsShape: _wishListsUIServices.SettingsShape(user, part.ContentItem.Id)
                    )));

            return Combined(shapes.ToArray());
        }
    }
}
