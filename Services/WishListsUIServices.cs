using Nwazet.Commerce.Models;
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
    public class WishListsUIServices : IWishListsUIServices {
        private readonly dynamic _shapeFactory;
        private readonly IEnumerable<IProductAttributesDriver> _attributesDrivers;
        private readonly IEnumerable<IWishListExtensionProvider> _wishListExtensionProviders;
        private readonly IWishListServices _wishListServices;

        public WishListsUIServices(
            IShapeFactory shapeFactory,
            IEnumerable<IProductAttributesDriver> attributesDrivers,
            IEnumerable<IWishListExtensionProvider> wishListExtensionProviders,
            IWishListServices wishListServices) {

            _shapeFactory = shapeFactory;
            _attributesDrivers = attributesDrivers;
            _wishListExtensionProviders = wishListExtensionProviders;
            _wishListServices = wishListServices;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        private string DefaultWishListTitle {
            get { return T("My WishList").Text; }
        }

        public dynamic CreateShape(IUser user, ProductPart product = null) {
            if (user == null) {
                return null;
            }

            var productId = 0;
            var attributeShapes = new List<dynamic>();
            if (product != null) {
                productId = product.ContentItem.Id;
                attributeShapes = _attributesDrivers
                .Select(p => p.GetAttributeDisplayShape(product.ContentItem, _shapeFactory))
                .ToList();
            }
            //get the additional shapes from the extension providers
            var creationShapes = new List<dynamic>();
            //process extensions
            foreach (var ext in _wishListExtensionProviders) {
                creationShapes.Add(ext.BuildCreationShape(user, product));
            }

            return _shapeFactory.CreateNewWishList(
                ProductId: productId,
                AttributeShapes: attributeShapes,
                CreationShapes: creationShapes,
                WishListTitle: DefaultWishListTitle
                );
        }

        public dynamic SettingsShape(IUser user, int wishListId = 0) {
            if (user == null) {
                return null;
            }
            //build the settings shape for each wishlist
            var settingsShapes = new List<dynamic>();
            var wishlists = _wishListServices.GetWishLists(user);
            foreach (var ext in _wishListExtensionProviders) {
                settingsShapes.Add(ext.BuildSettingsShape(wishlists));
            }

            return _shapeFactory.WishListsSettings(
                WishLists: wishlists,
                WishListId: wishListId,
                SettingsShapes: settingsShapes
                );
        }
    }
}
