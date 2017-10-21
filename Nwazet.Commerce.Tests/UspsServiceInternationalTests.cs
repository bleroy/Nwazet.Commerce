using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.Tests.Stubs;

namespace Nwazet.Commerce.Tests {
    [TestFixture]
    public class UspsServiceInternationalTests {
        private static UspsService BuildFakeUspsService() {
            return new UspsService(
                new FakeUspsWebService(),
                new WorkContextAccessorStub(null),
                new[] {new CoreShippingAreas()});
        }

        [Test]
        public void InternationalRequestDocumentIsCorrectlyBuilt() {
            var requestDocument = UspsService.BuildInternationalShippingRequestDocument(
                "Joe User", 45.3, 1030.54M, "Big Box", "France", 5, 3, 2, "90220", false, false,
                false, false, false, false, false);

            Assert.That(requestDocument.Name.LocalName, Is.EqualTo("IntlRateV2Request"));
            Assert.That(requestDocument.Attribute("USERID").Value, Is.EqualTo("Joe User"));
            Assert.That(requestDocument.Element("Revision").Value, Is.EqualTo("2"));

            var package = requestDocument.Element("Package");

            Assert.That(package, Is.Not.Null);
            Assert.That(package.Element("Pounds").Value, Is.EqualTo("2"));
            Assert.That(package.Element("Ounces").Value, Is.EqualTo("14"));
            Assert.That(package.Element("Machinable").Value, Is.EqualTo("false"));
            Assert.That(package.Element("MailType").Value, Is.EqualTo("Package"));
            Assert.That(package.Element("ValueOfContents").Value, Is.EqualTo("1030.54"));
            Assert.That(package.Element("Country").Value, Is.EqualTo("France"));
            Assert.That(package.Element("Container").Value, Is.EqualTo("Big Box"));
            Assert.That(package.Element("Size").Value, Is.EqualTo("REGULAR"));
            Assert.That(package.Element("Length").Value, Is.EqualTo("5"));
            Assert.That(package.Element("Width").Value, Is.EqualTo("3"));
            Assert.That(package.Element("Height").Value, Is.EqualTo("2"));
            Assert.That(package.Element("Girth").Value, Is.EqualTo("0"));
            Assert.That(package.Element("OriginZip").Value, Is.EqualTo("90220"));

            var specialServices = package.Element("ExtraServices");
            Assert.That(specialServices, Is.Null);
        }

        [Test]
        public void InternationalRequestSmallPackageIsNotMachinable() {
            var requestDocument = UspsService.BuildInternationalShippingRequestDocument(
                "Joe User", 45.3, 1030.54M, "Big Box", "France", 5, 3, 2, "90220");

            var package = requestDocument.Element("Package");
            Assert.That(package.Element("Machinable").Value, Is.EqualTo("false"));

            requestDocument = UspsService.BuildInternationalShippingRequestDocument(
                "Joe User", 45.3, 1030.54M, "Big Box", "France", 6, 1, 2, "90220",
                false, false, false, false, false);
            package = requestDocument.Element("Package");
            Assert.That(package.Element("Machinable").Value, Is.EqualTo("false"));

            requestDocument = UspsService.BuildInternationalShippingRequestDocument(
                "Joe User", 45.3, 1030.54M, "Big Box", "France", 6, 0, 3, "90220",
                false, false, false, false, false);
            package = requestDocument.Element("Package");
            Assert.That(package.Element("Machinable").Value, Is.EqualTo("false"));
        }

        [Test]
        public void InternationalRequestLargePackageIsNotMachinable() {
            var requestDocument = UspsService.BuildInternationalShippingRequestDocument(
                "Joe User", 45.3, 1030.54M, "Big Box", "France", 28, 17, 17, "90220");
            var package = requestDocument.Element("Package");
            Assert.That(package.Element("Machinable").Value, Is.EqualTo("false"));

            requestDocument = UspsService.BuildInternationalShippingRequestDocument(
                "Joe User", 45.3, 1030.54M, "Big Box", "France", 27, 18, 17, "90220");
            package = requestDocument.Element("Package");
            Assert.That(package.Element("Machinable").Value, Is.EqualTo("false"));

            requestDocument = UspsService.BuildInternationalShippingRequestDocument(
                "Joe User", 45.3, 1030.54M, "Big Box", "France", 27, 17, 18, "90220");
            package = requestDocument.Element("Package");
            Assert.That(package.Element("Machinable").Value, Is.EqualTo("false"));
        }

        [Test]
        public void InternationalRequestLightPackageIsNotMachinable() {
            var requestDocument = UspsService.BuildInternationalShippingRequestDocument(
                "Joe User", 5, 1030.54M, "Big Box", "France", 28, 17, 17, "90220");
            var package = requestDocument.Element("Package");
            Assert.That(package.Element("Machinable").Value, Is.EqualTo("false"));
        }

        [Test]
        public void InternationalRequestHeavyPackageIsNotMachinable() {
            var requestDocument = UspsService.BuildInternationalShippingRequestDocument(
                "Joe User", 401, 1030.54M, "Big Box", "France", 28, 17, 17, "90220");
            var package = requestDocument.Element("Package");
            Assert.That(package.Element("Machinable").Value, Is.EqualTo("false"));
        }

        [Test]
        public void InternationalRequestMachinablePackageIsMachinable() {
            var requestDocument = UspsService.BuildInternationalShippingRequestDocument(
                "Joe User", 45.3, 1030.54M, "Big Box", "France", 27, 17, 17, "90220");
            var package = requestDocument.Element("Package");
            Assert.That(package.Element("Machinable").Value, Is.EqualTo("true"));

            requestDocument = UspsService.BuildInternationalShippingRequestDocument(
                "Joe User", 45.3, 1030.54M, "Big Box", "France", 6, 1, 3, "90220");
            package = requestDocument.Element("Package");
            Assert.That(package.Element("Machinable").Value, Is.EqualTo("true"));

            requestDocument = UspsService.BuildInternationalShippingRequestDocument(
                "Joe User", 45.3, 1030.54M, "Big Box", "France", 10, 10, 10, "90220");
            package = requestDocument.Element("Package");
            Assert.That(package.Element("Machinable").Value, Is.EqualTo("true"));

            requestDocument = UspsService.BuildInternationalShippingRequestDocument(
                "Joe User", 6, 1030.54M, "Big Box", "France", 10, 10, 10, "90220");
            package = requestDocument.Element("Package");
            Assert.That(package.Element("Machinable").Value, Is.EqualTo("true"));

            requestDocument = UspsService.BuildInternationalShippingRequestDocument(
                "Joe User", 400, 1030.54M, "Big Box", "France", 10, 10, 10, "90220");
            package = requestDocument.Element("Package");
            Assert.That(package.Element("Machinable").Value, Is.EqualTo("true"));
        }

        [Test]
        public void InternationalRequestLargePackageIsLarge() {
            var requestDocument = UspsService.BuildInternationalShippingRequestDocument(
                "Joe User", 40, 1030.54M, "Big Box", "France", 13, 12, 12, "90220");
            var package = requestDocument.Element("Package");
            Assert.That(package.Element("Size").Value, Is.EqualTo("LARGE"));

            requestDocument = UspsService.BuildInternationalShippingRequestDocument(
                "Joe User", 40, 1030.54M, "Big Box", "France", 12, 13, 12, "90220");
            package = requestDocument.Element("Package");
            Assert.That(package.Element("Size").Value, Is.EqualTo("LARGE"));

            requestDocument = UspsService.BuildInternationalShippingRequestDocument(
                "Joe User", 40, 1030.54M, "Big Box", "France", 12, 12, 13, "90220");
            package = requestDocument.Element("Package");
            Assert.That(package.Element("Size").Value, Is.EqualTo("LARGE"));
        }

        [Test]
        public void InternationalRequestRegularPackageIsNotLarge() {
            var requestDocument = UspsService.BuildInternationalShippingRequestDocument(
                "Joe User", 40, 1030.54M, "Big Box", "France", 12, 12, 12, "90220");
            var package = requestDocument.Element("Package");
            Assert.That(package.Element("Size").Value, Is.EqualTo("REGULAR"));

            requestDocument = UspsService.BuildInternationalShippingRequestDocument(
                "Joe User", 40, 1030.54M, "Big Box", "France", 5, 5, 5, "90220");
            package = requestDocument.Element("Package");
            Assert.That(package.Element("Size").Value, Is.EqualTo("REGULAR"));
        }

        [Test]
        public void InternationalRegisteredMailIsExpressed() {
            var requestDocument = UspsService.BuildInternationalShippingRequestDocument(
                "Joe User", 45.3, 1030.54M, "Big Box", "France", 5, 3, 2, "90220",
                false, false, true, false, false, false, false);

            var package = requestDocument.Element("Package");
            var specialServices = package.Element("ExtraServices");
            Assert.That(specialServices, Is.Not.Null);
            Assert.That(specialServices.Element("ExtraService").Value, Is.EqualTo("0"));
        }

        [Test]
        public void InternationalInsuranceIsExpressed() {
            var requestDocument = UspsService.BuildInternationalShippingRequestDocument(
                "Joe User", 45.3, 1030.54M, "Big Box", "France", 5, 3, 2, "90220",
                false, false, false, true, false, false, false);

            var package = requestDocument.Element("Package");
            var specialServices = package.Element("ExtraServices");
            Assert.That(specialServices, Is.Not.Null);
            Assert.That(specialServices.Element("ExtraService").Value, Is.EqualTo("1"));
        }

        [Test]
        public void InternationalReturnReceiptIsExpressed() {
            var requestDocument = UspsService.BuildInternationalShippingRequestDocument(
                "Joe User", 45.3, 1030.54M, "Big Box", "France", 5, 3, 2, "90220",
                false, false, false, false, true, false, false);

            var package = requestDocument.Element("Package");
            var specialServices = package.Element("ExtraServices");
            Assert.That(specialServices, Is.Not.Null);
            Assert.That(specialServices.Element("ExtraService").Value, Is.EqualTo("2"));
        }

        [Test]
        public void InternationalCertificateOfMailingIsExpressed() {
            var requestDocument = UspsService.BuildInternationalShippingRequestDocument(
                "Joe User", 45.3, 1030.54M, "Big Box", "France", 5, 3, 2, "90220",
                false, false, false, false, false, true, false);

            var package = requestDocument.Element("Package");
            var specialServices = package.Element("ExtraServices");
            Assert.That(specialServices, Is.Not.Null);
            Assert.That(specialServices.Element("ExtraService").Value, Is.EqualTo("6"));
        }

        [Test]
        public void InternationalElectronicConfirmationIsExpressed() {
            var requestDocument = UspsService.BuildInternationalShippingRequestDocument(
                "Joe User", 45.3, 1030.54M, "Big Box", "France", 5, 3, 2, "90220",
                false, false, false, false, false, false, true);

            var package = requestDocument.Element("Package");
            var specialServices = package.Element("ExtraServices");
            Assert.That(specialServices, Is.Not.Null);
            Assert.That(specialServices.Element("ExtraService").Value, Is.EqualTo("9"));
        }

        [Test]
        public void InternationalServicesAreExpressed() {
            var requestDocument = UspsService.BuildInternationalShippingRequestDocument(
                "Joe User", 45.3, 1030.54M, "Big Box", "France", 5, 3, 2, "90220",
                false, false, true, true, true, true, true);

            var package = requestDocument.Element("Package");
            var specialServices = package.Element("ExtraServices");
            Assert.That(specialServices, Is.Not.Null);
            var values = specialServices
                .Elements("ExtraService")
                .Select(el => el.Value)
                .ToList();
            Assert.That(values.Count, Is.EqualTo(5));
            Assert.That(values, Is.EquivalentTo(new[] {"0", "1", "2", "6", "9"}));
        }

        [Test]
        public void InternationalServicePricesReturnsInternationalPrices() {
            var uspsService = BuildFakeUspsService();
            var prices = uspsService.Prices(
                "Joe User", 45.3, 1030.54M, "Big Box", null, null,
                "France", 5, 3, 2, "98052", null, false, false,
                false, false, false, false, false).ToList();
            Assert.That(prices.Count, Is.EqualTo(17));
            Assert.That(prices, new CollectionEquivalentConstraint(new[] {
                new ShippingOption {Description = "Global Express Guaranteed<sup>®</sup> (GXG)**; 1 - 3 business days", Price = 117.05M},
                new ShippingOption {Description = "USPS GXG<sup>™</sup> Envelopes**; 1 - 3 business days", Price = 117.05M},
                new ShippingOption {Description = "Express Mail<sup>®</sup> International; 3 - 5 business days", Price = 96.30M},
                new ShippingOption {Description = "Priority Mail<sup>®</sup> International; 6 - 10 business days", Price = 62.15M},
                new ShippingOption {Description = "USPS GXG<sup>™</sup> Envelopes**; 1 - 3 business days", Price = 104.50M},
                new ShippingOption {Description = "Express Mail<sup>®</sup> International; 3 - 5 business days", Price = 46.00M},
                new ShippingOption {Description = "Express Mail<sup>®</sup> International Flat Rate Envelope; 3 - 5 business days", Price = 44.95M},
                new ShippingOption {Description = "Express Mail<sup>®</sup> International Legal Flat Rate Envelope; 3 - 5 business days", Price = 44.95M},
                new ShippingOption {Description = "Express Mail<sup>®</sup> International Padded Flat Rate Envelope; 3 - 5 business days", Price = 44.95M},
                new ShippingOption {Description = "Priority Mail<sup>®</sup> International; 6 - 10 business days", Price = 36.75M},
                new ShippingOption {Description = "Priority Mail<sup>®</sup> International Flat Rate Envelope**; 6 - 10 business days", Price = 23.95M},
                new ShippingOption {Description = "Priority Mail<sup>®</sup> International Legal Flat Rate Envelope**; 6 - 10 business days", Price = 23.95M},
                new ShippingOption {Description = "Priority Mail<sup>®</sup> International Padded Flat Rate Envelope**; 6 - 10 business days", Price = 23.95M},
                new ShippingOption {Description = "Priority Mail<sup>®</sup> International Gift Card Flat Rate Envelope**; 6 - 10 business days", Price = 23.95M},
                new ShippingOption {Description = "Priority Mail<sup>®</sup> International Small Flat Rate Envelope**; 6 - 10 business days", Price = 23.95M},
                new ShippingOption {Description = "Priority Mail<sup>®</sup> International Window Flat Rate Envelope**; 6 - 10 business days", Price = 23.95M},
                new ShippingOption {Description = "First-Class Mail<sup>®</sup> International Letter**; Varies by country", Price = 2.70M}
            }).Using(new ShippingOption.ShippingOptionComparer()));
        }

        [Test]
        public void InternationalServicePricesWithValidationExpressionSelectsMatchingMethods() {
            var uspsService = BuildFakeUspsService();
            var prices = uspsService.Prices(
                "Joe User", 45.3, 1030.54M, "Big Box", "GXG", null,
                "France", 5, 3, 2, "98052", null, false, false,
                false, false, false, false, false).ToList();
            Assert.That(prices.Count, Is.EqualTo(3));
            Assert.That(prices, new CollectionEquivalentConstraint(new[] {
                new ShippingOption {Description = "Global Express Guaranteed<sup>®</sup> (GXG)**; 1 - 3 business days", Price = 117.05M},
                new ShippingOption {Description = "USPS GXG<sup>™</sup> Envelopes**; 1 - 3 business days", Price = 117.05M},
                new ShippingOption {Description = "USPS GXG<sup>™</sup> Envelopes**; 1 - 3 business days", Price = 104.50M}
            }).Using(new ShippingOption.ShippingOptionComparer()));
        }

        [Test]
        public void InternationalServicePricesWithExclusionExpressionExcludesMatchingMethods() {
            var uspsService = BuildFakeUspsService();
            var prices = uspsService.Prices(
                "Joe User", 45.3, 1030.54M, "Big Box", null, "Mail",
                "France", 5, 3, 2, "98052", null, false, false,
                false, false, false, false, false).ToList();
            Assert.That(prices.Count, Is.EqualTo(3));
            Assert.That(prices, new CollectionEquivalentConstraint(new[] {
                new ShippingOption {Description = "Global Express Guaranteed<sup>®</sup> (GXG)**; 1 - 3 business days", Price = 117.05M},
                new ShippingOption {Description = "USPS GXG<sup>™</sup> Envelopes**; 1 - 3 business days", Price = 117.05M},
                new ShippingOption {Description = "USPS GXG<sup>™</sup> Envelopes**; 1 - 3 business days", Price = 104.50M}
            }).Using(new ShippingOption.ShippingOptionComparer()));
        }

        [Test]
        public void InternationalServicePricesWithExclusionAndValidationExpressionsYieldsTheRightMethods() {
            var uspsService = BuildFakeUspsService();
            var prices = uspsService.Prices(
                "Joe User", 45.3, 1030.54M, "Big Box", "USPS", "Mail",
                "France", 5, 3, 2, "98052", null, false, false,
                false, false, false, false, false).ToList();
            Assert.That(prices.Count, Is.EqualTo(2));
            Assert.That(prices, new CollectionEquivalentConstraint(new[] {
                new ShippingOption {Description = "USPS GXG<sup>™</sup> Envelopes**; 1 - 3 business days", Price = 117.05M},
                new ShippingOption {Description = "USPS GXG<sup>™</sup> Envelopes**; 1 - 3 business days", Price = 104.50M}
            }).Using(new ShippingOption.ShippingOptionComparer()));
        }

        [Test]
        public void InternationalServicePricesWithRegisteredMailYieldsBumpedPrice() {
            var uspsService = BuildFakeUspsService();
            var price = uspsService.Prices(
                "Joe User", 45.3, 1030.54M, "Big Box", null, null,
                "France", 5, 3, 2, "98052", null, false, false,
                true, false, false, false, false).First();
            Assert.That(price.Price, Is.EqualTo(117.05 + 1.50));
            Assert.That(price.Description, Is.EqualTo("Global Express Guaranteed<sup>®</sup> (GXG)**; 1 - 3 business days"));
        }

        [Test]
        public void InternationalServicePricesWithInsuranceYieldsBumpedPrice() {
            var uspsService = BuildFakeUspsService();
            var price = uspsService.Prices(
                "Joe User", 45.3, 1030.54M, "Big Box", null, null,
                "France", 5, 3, 2, "98052", null, false, false,
                false, true, false, false, false).First();
            Assert.That(price.Price, Is.EqualTo(117.05 + 1.00));
            Assert.That(price.Description, Is.EqualTo("Global Express Guaranteed<sup>®</sup> (GXG)**; 1 - 3 business days"));
        }

        [Test]
        public void InternationalServicePricesWithReturnReceiptYieldsBumpedPrice() {
            var uspsService = BuildFakeUspsService();
            var price = uspsService.Prices(
                "Joe User", 45.3, 1030.54M, "Big Box", null, null,
                "France", 5, 3, 2, "98052", null, false, false,
                false, false, true, false, false).First();
            Assert.That(price.Price, Is.EqualTo(117.05 + 1.70));
            Assert.That(price.Description, Is.EqualTo("Global Express Guaranteed<sup>®</sup> (GXG)**; 1 - 3 business days"));
        }

        [Test]
        public void InternationalServicePricesWithCertificateOfMailingYieldsBumpedPrice() {
            var uspsService = BuildFakeUspsService();
            var price = uspsService.Prices(
                "Joe User", 45.3, 1030.54M, "Big Box", null, null,
                "France", 5, 3, 2, "98052", null, false, false,
                false, false, false, true, false).First();
            Assert.That(price.Price, Is.EqualTo(117.05 + 1.18));
            Assert.That(price.Description, Is.EqualTo("Global Express Guaranteed<sup>®</sup> (GXG)**; 1 - 3 business days"));
        }

        [Test]
        public void InternationalServicePricesWithConfirmationYieldsBumpedPrice() {
            var uspsService = BuildFakeUspsService();
            var price = uspsService.Prices(
                "Joe User", 45.3, 1030.54M, "Big Box", null, null,
                "France", 5, 3, 2, "98052", null, false, false,
                false, false, false, false, true).First();
            Assert.That(price.Price, Is.EqualTo(117.05 + 1.04));
            Assert.That(price.Description, Is.EqualTo("Global Express Guaranteed<sup>®</sup> (GXG)**; 1 - 3 business days"));
        }

        [Test]
        public void InternationalServicePricesWithSeveralOptionsYieldsCombinedPrice() {
            var uspsService = BuildFakeUspsService();
            var price = uspsService.Prices(
                "Joe User", 45.3, 1030.54M, "Big Box", null, null,
                "France", 5, 3, 2, "98052", null, false, false,
                true, true, false, true, false).First();
            Assert.That(price.Price, Is.EqualTo(117.05 + 1.50 + 1.00 + 1.18));
            Assert.That(price.Description, Is.EqualTo("Global Express Guaranteed<sup>®</sup> (GXG)**; 1 - 3 business days"));
        }
    }
}