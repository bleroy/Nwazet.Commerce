using Nwazet.Commerce.Models;
using Orchard;

namespace Nwazet.Commerce.Services {
    public interface IGoogleCheckoutService : ICheckoutService {
        GoogleCheckoutSettingsPart GetSettings();
    }
}
