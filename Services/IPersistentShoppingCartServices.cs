using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
using Orchard;
using Orchard.Security;

namespace Nwazet.Commerce.Services {
    public interface IPersistentShoppingCartServices : IDependency {

        List<ShoppingCartItem> RetrieveCartItems();
        string Country { get; set; }
        string ZipCode { get; set; }
        ShippingOption ShippingOption { get; set; }

        ProductsListPart GetCartForUser(IUser user);
        ProductsListPart CreateCartForUser(IUser user);

        ShoppingCartItem FindCartItem(IEnumerable<ShoppingCartItem> items, int productId, IDictionary<int, ProductAttributeValueExtended> attributeIdsToValues = null);
        /// <summary>
        /// Add the item to the cart. If the same item is already in the cart, its quantity is incremented.
        /// </summary>
        /// <param name="item">The item to add.</param>
        void AddItem(ShoppingCartItem item);
        /// <summary>
        /// Remove from the cart the item identified by the parameters
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="attributeIdsToValues"></param>
        void RemoveItem(int productId, IDictionary<int, ProductAttributeValueExtended> attributeIdsToValues = null);
        /// <summary>
        /// Remove all items from the cart.
        /// </summary>
        void ClearCart();
        //Remove from the cart all items whose quantity is set to zero.
        void ConsolidateCart();
    }
}
