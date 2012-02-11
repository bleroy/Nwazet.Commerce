using Nwazet.Commerce.Models;
using Orchard.ContentManagement.Drivers;

namespace Nwazet.Commerce.Drivers {
    public class ShoppingCartWidgetDriver : ContentPartDriver<ShoppingCartWidgetPart>
    {
        private readonly IShoppingCart _shoppingCart;

        public ShoppingCartWidgetDriver(IShoppingCart shoppingCart) {
            _shoppingCart = shoppingCart;
        }

        protected override DriverResult Display(ShoppingCartWidgetPart part, string displayType, dynamic shapeHelper) 
        {

            return ContentShape("ShoppingCartWidget", () => shapeHelper.ShoppingCartWidget(
                ItemCount: _shoppingCart.ItemCount(),
                TotalAmount: _shoppingCart.Total(),
                ContentItem: part.ContentItem
            ));
        }
    }
}