using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nwazet.Commerce.Extensions {
    public static class AttributeNameUtilities {

        public static string VersionName(string displayName) {
            var i = displayName.Length - 1;
            while (i >= 0 && char.IsDigit(displayName, i)) {
                i--;
            }

            var substring = i != displayName.Length - 1 ? displayName.Substring(i + 1) : string.Empty;
            int version;

            if (int.TryParse(substring, out version)) {
                displayName = displayName.Remove(displayName.Length - substring.Length);
                version = version > 0 ? ++version : 2;
            }
            else {
                version = 2;
            }

            return displayName + version;
        }
    }
}
