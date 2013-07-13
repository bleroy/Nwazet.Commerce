using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using Nwazet.Commerce.Models;
using Orchard;

namespace Nwazet.Commerce.Services {
    public class ShippingService {
        private const string EncryptionPurpose = "Nwazet Commerce Shipping Option";

        public static IEnumerable<ShippingOption> GetShippingOptions(
            IEnumerable<IShippingMethod> shippingMethods,
            IEnumerable<ShoppingCartQuantityProduct> productQuantities,
            string country,
            string zipCode,
            IWorkContextAccessor workContextAccessor) {

            var methods = shippingMethods.ToList();
            var quantities = productQuantities.ToList();
            var alreadyFound = new HashSet<ShippingOption>(new ShippingOption.ShippingOptionComparer());

            foreach (var method in methods) {
                var shippingOptions = method.ComputePrice(
                    quantities, methods, country, zipCode, workContextAccessor);
                foreach (var shippingOption in shippingOptions) {
                    shippingOption.ShippingCompany = method.ShippingCompany;
                    // Prepare a crypto version of the option so we can round-trip an option
                    // without having to re-query the web service or whatever generated the option.
                    var binaryValue = Encoding.UTF8.GetBytes(
                                          "Description:" + shippingOption.Description +
                                          "\nCompany:" + shippingOption.ShippingCompany +
                                          "\nIncluded:" + String.Join(",", shippingOption.IncludedShippingAreas ?? new string[0]) +
                                          "\nExcluded:" + String.Join(",", shippingOption.ExcludedShippingAreas ?? new string[0]) +
                                          "\nPrice:" + shippingOption.Price);
                    shippingOption.FormValue = Convert.ToBase64String(MachineKey.Protect(binaryValue, EncryptionPurpose));
                    if (!alreadyFound.Contains(shippingOption)) {
                        alreadyFound.Add(shippingOption);
                        yield return shippingOption;
                    }
                }
            }
        }

        public static ShippingOption RebuildShippingOption(string base64String) {
            var bytes = Convert.FromBase64String(base64String);
            var unprotectedBytes = MachineKey.Unprotect(bytes, EncryptionPurpose);
            if (unprotectedBytes == null) return null;
            var serializedOption = Encoding.UTF8.GetString(unprotectedBytes);
            var properties = serializedOption.Split('\n');
            var shippingOption = new ShippingOption {
                Description = properties[0].Substring(12),
                ShippingCompany = properties[1].Substring(8),
                IncludedShippingAreas = properties[2].Substring(9).Split(','),
                ExcludedShippingAreas = properties[3].Substring(9).Split(','),
                Price = double.Parse(properties[4].Substring(6))
            };
            return shippingOption;
        }
    }
}
