using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Orchard;

namespace Nwazet.Commerce.Services {
    public abstract class AnonymousIdentityBaseProvider : IAnonymousIdentityProvider {

        private readonly IWorkContextAccessor _wca;

        public AnonymousIdentityBaseProvider(
            IWorkContextAccessor wca) {

            _wca = wca;
        }

        public virtual string GetAnonymousIdentifier() {
            return GetHttpContext().Session.SessionID;
        }
        private HttpContextBase GetHttpContext() {
            var context = _wca.GetContext().HttpContext;
            if (context == null || context.Session == null) {
                throw new InvalidOperationException(
                    "ShoppingCartSessionStorage unavailable if session state can't be acquired.");
            }
            return context;
        }

    }
}
