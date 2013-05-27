using System;
using System.Runtime.Serialization;

namespace Nwazet.Commerce.Models {
    public class UspsException : Exception {
        public UspsException() : base() {
        }

        public UspsException(string message) : base(message) {
        }

        public UspsException(string message, Exception inner) : base(message, inner) {
        }

        protected UspsException(SerializationInfo info, StreamingContext context) : base(info, context) {
        }
    }
}
