using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Nwazet.Commerce.ViewModels;
using Nwazet.Commerce.Services;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Nwazet.WishLists")]
    public class WishListListPartDriver : ContentPartDriver<WishListListPart> {
        private readonly IContentManager _contentManager;
        private readonly IEnumerable<IWishListExtensionProvider> _wishListExtensionProviders;

        public WishListListPartDriver(
            IContentManager contentManager,
            IEnumerable<IWishListExtensionProvider> wishListExtensionProviders) {

            _contentManager = contentManager;
            _wishListExtensionProviders = wishListExtensionProviders;
        }

        protected override string Prefix {
            get { return "NwazetCommerceWishList"; }
        }

        protected override DriverResult Display(WishListListPart part, string displayType, dynamic shapeHelper) {
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

            return ContentShape("Parts_WishListList", () =>
                shapeHelper.Parts_WishListList(new WishListListViewModel {
                    ElementsShapes = elementsShapes,
                    ExtensionShapes = extensionsShapes
                }));
        }
    }
}
