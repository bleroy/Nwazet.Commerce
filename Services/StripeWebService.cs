using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using Nwazet.Commerce.Exceptions;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Stripe")]
    public class StripeWebService : IStripeWebService {
        private const string UrlFormat = "https://api.stripe.com/v1/";

        public JObject Query(string secretKey, string serviceName, NameValueCollection parameters) {
            var serviceUrl = UrlFormat + serviceName;
            var client = new WebClient {
                Credentials = new NetworkCredential(secretKey, "")
            };
            byte[] responseBytes;
            try {
                responseBytes = client.UploadValues(serviceUrl, "POST", parameters);
            }
            catch (WebException ex) {
                throw new StripeException(ex.Message, ex.InnerException) {
                    Status = ex.Status,
                    Response = ex.Response
                };
            }
            var responseText = Encoding.UTF8.GetString(responseBytes);
            var responseObject = JObject.Parse(responseText);
            return responseObject;
        }
    }
}