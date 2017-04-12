using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
using Orchard.Utility.Extensions;

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

        public static string GenerateAttributeTechnicalName(ProductAttributePart part, IEnumerable<ProductAttributePart> partsToCheck) {
            return GenerateAttributeTechnicalName(part.DisplayName.ToSafeName(), partsToCheck);
        }

        public static string GenerateAttributeTechnicalName(string tName, IEnumerable<ProductAttributePart> partsToCheck) {
            tName = tName.ToSafeName();
            while (partsToCheck.Any(eap =>
                    string.Equals(eap.TechnicalName.Trim(), tName.Trim(), StringComparison.OrdinalIgnoreCase))) {
                tName = AttributeNameUtilities.VersionName(tName);
            }
            return tName;
        }
    }
}
