using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.Commerce")]
    public class CommerceLocalStorageSettings : ILocalStorageSettings {
        public bool UseLocalStorage() {
            return true;
        }
    }
}
