using Nwazet.Commerce.Models;

namespace Nwazet.Commerce.Services {
    public interface IAddressFormatter {
        string Format(Address address);
    }
}
