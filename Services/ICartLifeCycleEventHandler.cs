using Nwazet.Commerce.Models;
using Orchard.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nwazet.Commerce.Services {
    public interface ICartLifeCycleEventHandler : IEventHandler {
        void Updated();
        void ItemAdded(ShoppingCartItem item);
        void ItemRemoved(ShoppingCartItem item);
        void Finalized();
        void Updated(IEnumerable<ShoppingCartItem> addedItems, IEnumerable<ShoppingCartItem> removedItems);
    }
}
