using System;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Nwazet.Commerce.Exceptions {
    public class StripeException : Exception {
        public StripeException(string message) : base(message) {}
        public StripeException(string message, Exception innerException) : base (message, innerException) {}

        public WebExceptionStatus Status { get; set; }
        public JObject Response { get; set; }
    }
}
