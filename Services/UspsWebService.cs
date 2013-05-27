using System;
using System.Net;
using System.Xml.Linq;

namespace Nwazet.Commerce.Services {
    public class UspsWebService : IUspsWebService {
        public XElement QueryWebService(Uri url) {
            var request = new WebClient();
            var response = request.DownloadString(url);
            return XElement.Parse(response);
        }
    }
}