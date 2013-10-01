using System.Collections.Generic;
using Orchard;

namespace Nwazet.Commerce.Services {
    public interface ITaxProvider : IDependency {
        string Name { get; }
        string ContentTypeName { get; }

        IEnumerable<ITax> GetTaxes();
    }
}
