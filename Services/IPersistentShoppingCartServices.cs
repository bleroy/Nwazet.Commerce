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

        PersistentShoppingCartPart GetCartForUser(IUser user);
        PersistentShoppingCartPart CreateCartForUser(IUser user);

        PersistentShoppingCartPart GetAnonymousCart();

        PersistentShoppingCartPart UpdateCountry(PersistentShoppingCartPart cart, string country);
        PersistentShoppingCartPart UpdateZipCode(PersistentShoppingCartPart cart, string zipCode);
        PersistentShoppingCartPart UpdateShippingOption(PersistentShoppingCartPart cart, ShippingOption shippingOption);
    }
}
