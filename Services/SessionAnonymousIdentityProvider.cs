using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard;

namespace Nwazet.Commerce.Services {
    public class SessionAnonymousIdentityProvider : AnonymousIdentityBaseProvider {

        public SessionAnonymousIdentityProvider(IWorkContextAccessor wca) : base(wca) { }
    }
}
