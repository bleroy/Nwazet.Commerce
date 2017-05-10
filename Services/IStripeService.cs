using Nwazet.Commerce.Models;
using Nwazet.Commerce.ViewModels;

namespace Nwazet.Commerce.Services {
    public interface IStripeService : ICheckoutService {
        StripeSettingsPart GetSettings();
        StripeCheckoutViewModel DecryptCheckoutData(string checkoutData);
        CreditCardCharge Charge(string token, decimal amount);
        bool IsInTestMode();
    }
}
