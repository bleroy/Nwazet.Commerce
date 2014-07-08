using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Orchard;
using Orchard.Localization;

namespace Nwazet.Commerce.Services {
    public interface IOrderService : IDependency {
        OrderPart CreateOrder(
            CreditCardCharge creditCardCharge,
            IEnumerable<CheckoutItem> items,
            double subTotal,
            double total,
            TaxAmount taxes,
            ShippingOption shippingOption,
            Address shippingAddress,
            Address billingAddress,
            string customerEmail,
            string customerPhone,
            string specialInstructions,
            string status,
            string trackingUrl = null,            
            bool isTestOrder = false,
            int userId = -1,
            double amountPaid = 0,
            string purchaseOrder = "");

        OrderPart Get(int orderId);
        string GetDisplayUrl(OrderPart order);
        string GetEditUrl(OrderPart order);

        IDictionary<string, LocalizedString> StatusLabels { get; }
        IDictionary<string, LocalizedString> EventCategoryLabels { get; }
    }
}
