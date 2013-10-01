using System;
using System.Net;

namespace Nwazet.Commerce.Exceptions {
    public class StripeException : Exception {
        public StripeException(string message) : base(message) {}
        public StripeException(string message, Exception innerException) : base (message, innerException) {}

        public WebExceptionStatus Status { get; set; }
        public WebResponse Response { get; set; }
    }
}
