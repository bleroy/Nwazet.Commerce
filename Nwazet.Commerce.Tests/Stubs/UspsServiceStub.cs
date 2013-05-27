using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;

namespace Nwazet.Commerce.Tests.Stubs {
    internal class UspsServiceStub : IUspsService {
        private readonly string _uspsUserId;
        private readonly string _originZip;
        private readonly bool _commercialPrices;
        private readonly bool _commercialPlusPrices;
        private readonly double _price;

        public UspsServiceStub(string uspsUserId, string originZip, bool commercialPrices, bool commercialPlusPrices, double price = 10) {
            _uspsUserId = uspsUserId;
            _originZip = originZip;
            _commercialPrices = commercialPrices;
            _commercialPlusPrices = commercialPlusPrices;
            _price = price;
        }

        public IEnumerable<string> GetInternationalShippingAreas() {
            return new[] {"notusistan"};
        }

        public IEnumerable<string> GetDomesticShippingAreas() {
            return new[] {"us"};
        }

        public UspsSettingsPart GetSettings() {
            var settings = new UspsSettingsPart();
            Helpers.PreparePart<UspsSettingsPart, UspsSettingsPartRecord>(settings, "Site");
            settings.UserId = _uspsUserId;
            settings.OriginZip = _originZip;
            settings.CommercialPrices = _commercialPrices;
            settings.CommercialPlusPrices = _commercialPlusPrices;
            return settings;
        }

        public IEnumerable<ShippingOption> Prices(string userId, double weightInOunces, double valueOfContents, string container,
                                  string serviceNameValidationExpression, string serviceNameExclusionExpression, string country,
                                  int lengthInInches, int widthInInches, int heightInInches, string originZip, string destinationZip,
                                  bool commercialPrices, bool commercialPlusPrices, bool registeredMail, bool insurance,
                                  bool returnReceipt, bool certificateOfMailing, bool electronicConfirmation) {
            yield return new ShippingOption {
                Description = "Shipping method",
                Price = _price
            };
        }
    }
}
