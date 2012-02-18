using System.Collections.Generic;
using Orchard;

namespace Nwazet.Commerce.Services {
    public interface IExtraCartInfoProvider : IDependency {
        IEnumerable<string> GetExtraCartInfo();
    }
}
