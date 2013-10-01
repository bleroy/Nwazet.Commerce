using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
using Orchard;

namespace Nwazet.Commerce.Services {
    public interface IStripeWebService : IDependency {
        JObject Query(string secretKey, string serviceName, NameValueCollection parameters);
    }
}
