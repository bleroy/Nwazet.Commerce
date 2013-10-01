using Nwazet.Commerce.Models;
using Orchard;

namespace Nwazet.Commerce.Services {
    public interface IAddressFormatter : IDependency {
        string Format(Address address);
    }
}
