using Orchard.Environment.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.Commerce")]
    public class CommerceLocalStorageSettings : ILocalStorageSettings {
        public bool UseLocalStorage() {
            return true;
        }
    }
}
