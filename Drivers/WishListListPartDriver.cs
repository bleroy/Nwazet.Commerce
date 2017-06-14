using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;
using System.Collections.Generic;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Nwazet.WishLists")]
    public class WishListListPartDriver : ContentPartDriver<WishListListPart> {
        private readonly IContentManager _contentManager;
        private readonly IEnumerable<IWishListExtensionProvider> _wishListExtensionProviders;
        private readonly IWorkContextAccessor _wca;
        private readonly IWishListServices _wishListServices;

        public WishListListPartDriver(
            IContentManager contentManager,
            IEnumerable<IWishListExtensionProvider> wishListExtensionProviders,
            IWorkContextAccessor wca,
            IWishListServices wishListServices) {

            _contentManager = contentManager;
            _wishListExtensionProviders = wishListExtensionProviders;
            _wca = wca;
            _wishListServices = wishListServices;
        }

        protected override string Prefix {
            get { return "NwazetCommerceWishList"; }
        }

        protected override DriverResult Display(WishListListPart part, string displayType, dynamic shapeHelper) {
            var shapes = new List<DriverResult>();
            var user = _wca.GetContext().CurrentUser;
            //get the elements out of the wishlist
            List<dynamic> elementsShapes = new List<dynamic>();
            
            foreach (var element in part.WishListElements) {
                var elementPart = element.As<WishListElementPart>();
                if (elementPart != null) {
                    elementsShapes.Add(_contentManager.BuildDisplay(elementPart));
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
                    CreateShape: _wishListServices.CreateShape(user),
                    SettingsShape: _wishListServices.SettingsShape(user, part.ContentItem.Id)
                    )));

            return Combined(shapes.ToArray());
        }
    }
}
