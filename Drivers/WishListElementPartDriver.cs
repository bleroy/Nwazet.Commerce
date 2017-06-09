using Nwazet.Commerce.Models;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Nwazet.Commerce.Services;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Autoroute.Models;
using Orchard.MediaLibrary.Fields;
using Nwazet.Commerce.ViewModels;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Nwazet.WishLists")]
    public class WishListElementPartDriver : ContentPartDriver<WishListElementPart> {
        private readonly IEnumerable<IProductAttributesDriver> _attributeDrivers;
        private readonly IEnumerable<IProductAttributeExtensionProvider> _extensionProviders;
        private readonly IContentManager _contentManager;
        private readonly IPriceService _priceService;
        private readonly ICurrencyProvider _currencyProvider;
        private readonly IEnumerable<IWishListExtensionProvider> _wishListExtensionProviders;

        public WishListElementPartDriver(
            IEnumerable<IProductAttributeExtensionProvider> extensionProviders,
            IContentManager contentManager,
            IPriceService priceService,
            ICurrencyProvider currencyProvider,
            IEnumerable<IProductAttributesDriver> attributeDrivers,
            IEnumerable<IWishListExtensionProvider> wishListExtensionProviders) {

            _extensionProviders = extensionProviders;
            _contentManager = contentManager;
            _priceService = priceService;
            _currencyProvider = currencyProvider;
            _attributeDrivers = attributeDrivers;
            _wishListExtensionProviders = wishListExtensionProviders;
        }

        protected override string Prefix {
            get { return "NwazetCommerceWishListElement"; }
        }

        protected override DriverResult Display(WishListElementPart part, string displayType, dynamic shapeHelper) {
            var item = part.Item;
            //add attribute extension providers, if any
            if (item.AttributeIdsToValues != null) {
                foreach (var option in item.AttributeIdsToValues) {
                    option.Value.ExtensionProviderInstance = _extensionProviders.SingleOrDefault(e => e.Name == option.Value.ExtensionProvider);
                }
            }

            var product = _contentManager.Get<ProductPart>(item.ProductId, VersionOptions.Published,
                new QueryHints().ExpandParts<TitlePart, ProductPart, AutoroutePart>());

            var productDetails = _priceService.GetDiscountedPrice(
                new ShoppingCartQuantityProduct(item.Quantity, product, item.AttributeIdsToValues));


            var shapes = new List<DriverResult>();
            //base element shape
            //Additional shapes from extensions
            var extensionShapes = new List<dynamic>();
            foreach (var ext in _wishListExtensionProviders) {
                extensionShapes.Add(ext.BuildElementDisplayShape(part));
            }
            shapes.Add(ContentShape("Parts_WishListElement", () =>
                shapeHelper.Parts_WishListElement(new WishListElementViewModel {
                    Title = _contentManager.GetItemMetadata(product.ContentItem).DisplayText,
                    ProductAttributes = productDetails.AttributeIdsToValues,
                    ContentItem = product.ContentItem,
                    ProductImage = ((MediaLibraryPickerField)product.ContentItem.Parts.SelectMany(pa => pa.Fields).FirstOrDefault(field => field.Name == "ProductImage")),
                    IsDigital = product.IsDigital,
                    ConsiderInventory = product.ConsiderInventory,
                    Price = product.Price,
                    OriginalPrice = product.Price,
                    DiscountedPrice = productDetails.Price,
                    Inventory = product.Inventory,
                    AllowBackOrder = product.AllowBackOrder,
                    OutOfStockMessage = product.OutOfStockMessage,
                    CurrencyProvider = _currencyProvider,
                    ExtensionShapes = extensionShapes
                })));
            //get the shapes for the actions on the element
            //Add to cart
            if (product.Inventory > 0 || product.AllowBackOrder || (product.IsDigital && !product.ConsiderInventory)) {
                shapes.Add(ContentShape("Parts_Product_AddToCartFromWishList", () =>
                    shapeHelper.Parts_Product_AddToCartFromWishList(
                        ProductId: product.Id,
                        MinimumOrderQuantity: product.MinimumOrderQuantity,
                        AttributeIdsToValues: part.Item.AttributeIdsToValues)
                ));
            }
            //Remove from list
            shapes.Add(ContentShape("Parts_Product_RemoveFromWishList", () =>
                shapeHelper.Parts_Product_RemoveFromWishList(
                    WishListElementId: part.ContentItem.Id,
                    WishListId: part.WishListId
                    )
            ));
            

            return Combined(shapes.ToArray());
        }

    }
}
