using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Nwazet.Commerce.Models;
using Orchard;

namespace Nwazet.Commerce.Services {
    public interface IUspsService : IDependency {
        IEnumerable<string> GetInternationalShippingAreas();
        IEnumerable<string> GetDomesticShippingAreas();
        UspsSettingsPart GetSettings();

        IEnumerable<ShippingOption> Prices(
            string userId,
            double weightInOunces,
            decimal valueOfContents,
            string container,
            string serviceNameValidationExpression,
            string serviceNameExclusionExpression,
            string country,
            int lengthInInches,
            int widthInInches,
            int heightInInches,
            string originZip,
            string destinationZip,
            bool commercialPrices,
            bool commercialPlusPrices,
            bool registeredMail,
            bool insurance,
            bool returnReceipt,
            bool certificateOfMailing,
            bool electronicConfirmation
            );
    }
}