using System;
using System.Runtime.Serialization;

namespace Nwazet.Commerce.Models {
    public class ShippingException : Exception {
        public ShippingException() : base() {
        }

        public ShippingException(string message) : base(message) {
        }

        public ShippingException(string message, Exception inner) : base(message, inner) {
        }

        protected ShippingException(SerializationInfo info, StreamingContext context) : base(info, context) {
        }
    }
}
