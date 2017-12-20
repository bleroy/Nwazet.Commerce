using Orchard.Localization;
using System;

namespace Nwazet.Commerce.Exceptions {
    public class TerritoryInternalDuplicateException : Exception {
        public TerritoryInternalDuplicateException() :
            base("A territory with the same name already exists.") { }
        public TerritoryInternalDuplicateException(LocalizedString message) : base(message.Text) { }
        public TerritoryInternalDuplicateException(string message) : base(message) { }
        public TerritoryInternalDuplicateException(string message, Exception innerException) : base (message, innerException) { }
    }
}
