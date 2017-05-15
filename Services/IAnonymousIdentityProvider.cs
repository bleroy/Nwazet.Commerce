using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard;

namespace Nwazet.Commerce.Services {
    public interface IAnonymousIdentityProvider : IDependency {
        /// <summary>
        /// Gets a string that identifies the current anonymous client.
        /// </summary>
        /// <returns></returns>
        string GetAnonymousIdentifier();
        
    }
}
