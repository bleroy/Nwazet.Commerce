using Nwazet.Commerce.Models;

namespace Nwazet.Commerce.Services {
    public interface IStripeService : ICheckoutService {
        StripeSettingsPart GetSettings();
    }
}
