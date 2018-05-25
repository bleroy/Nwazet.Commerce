using Orchard;
using System.Collections.Generic;

namespace Nwazet.Commerce.Services {
    public interface ITaxProvider : IDependency {
        string Name { get; }
        string ContentTypeName { get; }

        IEnumerable<ITax> GetTaxes();
    }

    
}
