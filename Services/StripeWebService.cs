using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
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
                var exceptionResponse = ex.Response;
                var exceptionObject = exceptionResponse == null 
                    ? null 
                    : JObject.Parse(new StreamReader(exceptionResponse.GetResponseStream()).ReadToEnd());
                throw new StripeException(ex.Message, ex.InnerException) {
                    Status = ex.Status,
                    Response = exceptionObject
                };
            }
            var responseText = Encoding.UTF8.GetString(responseBytes);
            var responseObject = JObject.Parse(responseText);
            return responseObject;
        }
    }
}