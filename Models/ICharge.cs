using Orchard;

namespace Nwazet.Commerce.Models {
    public interface ICharge : IDependency {
        string TransactionId { get; }
        string ChargeText { get; }
        CheckoutError Error { get; set; }
        string ToString();
    }
}