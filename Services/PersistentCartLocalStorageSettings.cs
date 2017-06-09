using Orchard;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.PersistentCart")]
    public class PersistentCartLocalStorageSettings : ILocalStorageSettings {
        private readonly IWorkContextAccessor _wca;

        public PersistentCartLocalStorageSettings(IWorkContextAccessor wca) {
            _wca = wca;
        }
        public bool UseLocalStorage() {
            return _wca.GetContext().CurrentUser == null;
        }
    }
}
