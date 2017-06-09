using Orchard;

namespace Nwazet.Commerce.Services {
    public interface ILocalStorageSettings : IDependency {
        bool UseLocalStorage();
    }
}
