using Orchard.Environment.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
using Orchard;
using Orchard.Workflows.Services;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.Commerce")]
    public class CartLifeCycleEventHandler : ICartLifeCycleEventHandler {
        private readonly IWorkContextAccessor _wca;
        private readonly IWorkflowManager _workflowManager;
        private readonly IShoppingCart _shoppingCart;

        public CartLifeCycleEventHandler(
            IShoppingCart shoppingCart,
            IWorkContextAccessor wca,
            IWorkflowManager workflowManager) {

            _shoppingCart = shoppingCart;
            _wca = wca;
            _workflowManager = workflowManager;
        }
        public void Finalized() {
            _workflowManager.TriggerEvent("CartFinalized",
                _wca.GetContext().CurrentSite,
                () => new Dictionary<string, object> {
                    {"Cart", _shoppingCart}
                });
        }
        
        public void ItemAdded(ShoppingCartItem item) {
            Trigger("CartItemAdded", item);
            Updated();
        }

        
        public void ItemRemoved(ShoppingCartItem item) {
            Trigger("CartItemRemoved", item);
            Updated();
        }

        public void Updated() {
            _workflowManager.TriggerEvent("CartUpdated",
                _wca.GetContext().CurrentSite,
                () => new Dictionary<string, object> {
                    {"Cart", _shoppingCart}
                });
        }

        public void Updated(IEnumerable<ShoppingCartItem> addedItems, IEnumerable<ShoppingCartItem> removedItems) {
            foreach (var item in addedItems) {
                Trigger("CartItemAdded", item);
            }
            foreach (var item in removedItems) {
                Trigger("CartItemRemoved", item);
            }
            Updated();
        }

        private void Trigger(string activityName, ShoppingCartItem item) {
            _workflowManager.TriggerEvent(activityName,
                _wca.GetContext().CurrentSite,
                () => new Dictionary<string, object> {
                    {"Cart", _shoppingCart},
                    {"Item", item }
                });
        }
    }
}
