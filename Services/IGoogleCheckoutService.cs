using Nwazet.Commerce.Models;

namespace Nwazet.Commerce.Services {
    public interface IGoogleCheckoutService : ICheckoutService {
        GoogleCheckoutSettingsPart GetSettings();
    }
}
