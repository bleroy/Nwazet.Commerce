using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.Tests.Stubs;

namespace Nwazet.Commerce.Tests {
    [TestFixture]
    public class UspsServiceDomesticTests {
        private static UspsService BuildFakeUspsService() {
            return new UspsService(
                new FakeUspsWebService(),
                new WorkContextAccessorStub(null),
                new[] {new CoreShippingAreas()});
        }

        [Test]
        public void DomesticRequestDocumentIsCorrectlyBuilt() {
            var requestDocument = UspsService.BuildDomesticShippingRequestDocument(
                "Joe User", 45.3, 1030.54M, "Big Box", 5, 3, 2, "98052", "90220",
                false, false, false, false, false);

            Assert.That(requestDocument.Name.LocalName, Is.EqualTo("RateV4Request"));
            Assert.That(requestDocument.Attribute("USERID").Value, Is.EqualTo("Joe User"));
            Assert.That(requestDocument.Element("Revision").Value, Is.EqualTo("2"));

            var package = requestDocument.Element("Package");

            Assert.That(package, Is.Not.Null);
            Assert.That(package.Element("Service").Value, Is.EqualTo("ALL"));
            Assert.That(package.Element("FirstClassMailType").Value, Is.EqualTo("PARCEL"));
            Assert.That(package.Element("ZipOrigination").Value, Is.EqualTo("98052"));
            Assert.That(package.Element("ZipDestination").Value, Is.EqualTo("90220"));
            Assert.That(package.Element("Pounds").Value, Is.EqualTo("2"));
            Assert.That(package.Element("Ounces").Value, Is.EqualTo("14"));
            Assert.That(package.Element("Container").Value, Is.EqualTo("BIG BOX"));
            Assert.That(package.Element("Size").Value, Is.EqualTo("REGULAR"));
            Assert.That(package.Element("Length").Value, Is.EqualTo("5"));
            Assert.That(package.Element("Width").Value, Is.EqualTo("3"));
            Assert.That(package.Element("Height").Value, Is.EqualTo("2"));
            Assert.That(package.Element("Girth"), Is.Null);
            Assert.That(package.Element("Value").Value, Is.EqualTo((1030.54M).ToString()));
            Assert.That(package.Element("SortBy").Value, Is.EqualTo("PACKAGE"));
            Assert.That(package.Element("Machinable").Value, Is.EqualTo("false"));

            var specialServices = package.Element("SpecialService");
            Assert.That(specialServices, Is.Null);
        }

        [Test]
        public void DomesticRequestSmallPackageIsNotMachinable() {
            var requestDocument = UspsService.BuildDomesticShippingRequestDocument(
                "Joe User", 45.3, 1030.54M, "Big Box", 5, 1, 3, "98052", "90220",
                false, false, false, false, false);
            var package = requestDocument.Element("Package");
            Assert.That(package.Element("Machinable").Value, Is.EqualTo("false"));

            requestDocument = UspsService.BuildDomesticShippingRequestDocument(
                "Joe User", 45.3, 1030.54M, "Big Box", 6, 1, 2, "98052", "90220",
                false, false, false, false, false);
            package = requestDocument.Element("Package");
            Assert.That(package.Element("Machinable").Value, Is.EqualTo("false"));

            requestDocument = UspsService.BuildDomesticShippingRequestDocument(
                "Joe User", 45.3, 1030.54M, "Big Box", 6, 0, 3, "98052", "90220",
                false, false, false, false, false);
            package = requestDocument.Element("Package");
            Assert.That(package.Element("Machinable").Value, Is.EqualTo("false"));
        }

        [Test]
        public void DomesticRequestLargePackageIsNotMachinable() {
            var requestDocument = UspsService.BuildDomesticShippingRequestDocument(
                "Joe User", 45.3, 1030.54M, "Big Box", 28, 17, 17, "98052", "90220");
            var package = requestDocument.Element("Package");
            Assert.That(package.Element("Machinable").Value, Is.EqualTo("false"));

            requestDocument = UspsService.BuildDomesticShippingRequestDocument(
                "Joe User", 45.3, 1030.54M, "Big Box", 27, 18, 17, "98052", "90220");
            package = requestDocument.Element("Package");
            Assert.That(package.Element("Machinable").Value, Is.EqualTo("false"));

            requestDocument = UspsService.BuildDomesticShippingRequestDocument(
                "Joe User", 45.3, 1030.54M, "Big Box", 27, 17, 18, "98052", "90220");
            package = requestDocument.Element("Package");
            Assert.That(package.Element("Machinable").Value, Is.EqualTo("false"));
        }

        [Test]
        public void DomesticRequestLightPackageIsNotMachinable() {
            var requestDocument = UspsService.BuildDomesticShippingRequestDocument(
                "Joe User", 5, 1030.54M, "Big Box", 28, 17, 17, "98052", "90220");
            var package = requestDocument.Element("Package");
            Assert.That(package.Element("Machinable").Value, Is.EqualTo("false"));
        }

        [Test]
        public void DomesticRequestHeavyPackageIsNotMachinable() {
            var requestDocument = UspsService.BuildDomesticShippingRequestDocument(
                "Joe User", 401, 1030.54M, "Big Box", 28, 17, 17, "98052", "90220");
            var package = requestDocument.Element("Package");
            Assert.That(package.Element("Machinable").Value, Is.EqualTo("false"));
        }

        [Test]
        public void DomesticRequestMachinablePackageIsMachinable() {
            var requestDocument = UspsService.BuildDomesticShippingRequestDocument(
                "Joe User", 45.3, 1030.54M, "Big Box", 27, 17, 17, "98052", "90220");
            var package = requestDocument.Element("Package");
            Assert.That(package.Element("Machinable").Value, Is.EqualTo("true"));

            requestDocument = UspsService.BuildDomesticShippingRequestDocument(
                "Joe User", 45.3, 1030.54M, "Big Box", 6, 1, 3, "98052", "90220");
            package = requestDocument.Element("Package");
            Assert.That(package.Element("Machinable").Value, Is.EqualTo("true"));

            requestDocument = UspsService.BuildDomesticShippingRequestDocument(
                "Joe User", 45.3, 1030.54M, "Big Box", 10, 10, 10, "98052", "90220");
            package = requestDocument.Element("Package");
            Assert.That(package.Element("Machinable").Value, Is.EqualTo("true"));

            requestDocument = UspsService.BuildDomesticShippingRequestDocument(
                "Joe User", 6, 1030.54M, "Big Box", 10, 10, 10, "98052", "90220");
            package = requestDocument.Element("Package");
            Assert.That(package.Element("Machinable").Value, Is.EqualTo("true"));

            requestDocument = UspsService.BuildDomesticShippingRequestDocument(
                "Joe User", 400, 1030.54M, "Big Box", 10, 10, 10, "98052", "90220");
            package = requestDocument.Element("Package");
            Assert.That(package.Element("Machinable").Value, Is.EqualTo("true"));
        }

        [Test]
        public void DomesticRequestLargePackageIsLarge() {
            var requestDocument = UspsService.BuildDomesticShippingRequestDocument(
                "Joe User", 40, 1030.54M, "Big Box", 13, 12, 12, "98052", "90220");
            var package = requestDocument.Element("Package");
            Assert.That(package.Element("Size").Value, Is.EqualTo("LARGE"));

            requestDocument = UspsService.BuildDomesticShippingRequestDocument(
                "Joe User", 40, 1030.54M, "Big Box", 12, 13, 12, "98052", "90220");
            package = requestDocument.Element("Package");
            Assert.That(package.Element("Size").Value, Is.EqualTo("LARGE"));

            requestDocument = UspsService.BuildDomesticShippingRequestDocument(
                "Joe User", 40, 1030.54M, "Big Box", 12, 12, 13, "98052", "90220");
            package = requestDocument.Element("Package");
            Assert.That(package.Element("Size").Value, Is.EqualTo("LARGE"));
        }

        [Test]
        public void DomesticRequestRegularPackageIsNotLarge() {
            var requestDocument = UspsService.BuildDomesticShippingRequestDocument(
                "Joe User", 40, 1030.54M, "Big Box", 12, 12, 12, "98052", "90220");
            var package = requestDocument.Element("Package");
            Assert.That(package.Element("Size").Value, Is.EqualTo("REGULAR"));

            requestDocument = UspsService.BuildDomesticShippingRequestDocument(
                "Joe User", 40, 1030.54M, "Big Box", 5, 5, 5, "98052", "90220");
            package = requestDocument.Element("Package");
            Assert.That(package.Element("Size").Value, Is.EqualTo("REGULAR"));
        }

        [Test]
        public void DomesticRegisteredMailIsExpressed() {
            var requestDocument = UspsService.BuildDomesticShippingRequestDocument(
                "Joe User", 45.3, 1030.54M, "Big Box", 5, 3, 2, "98052", "90220",
                true, false, false, false, false);

            var package = requestDocument.Element("Package");
            var specialServices = package.Element("SpecialServices");
            Assert.That(specialServices, Is.Not.Null);
            Assert.That(specialServices.Element("SpecialService").Value, Is.EqualTo("4"));
        }

        [Test]
        public void DomesticInsuranceIsExpressed() {
            var requestDocument = UspsService.BuildDomesticShippingRequestDocument(
                "Joe User", 45.3, 1030.54M, "Big Box", 5, 3, 2, "98052", "90220",
                false, true, false, false, false);

            var package = requestDocument.Element("Package");
            var specialServices = package.Element("SpecialServices");
            Assert.That(specialServices, Is.Not.Null);
            Assert.That(specialServices.Element("SpecialService").Value, Is.EqualTo("1"));
        }

        [Test]
        public void DomesticReturnReceiptIsExpressed() {
            var requestDocument = UspsService.BuildDomesticShippingRequestDocument(
                "Joe User", 45.3, 1030.54M, "Big Box", 5, 3, 2, "98052", "90220",
                false, false, true, false, false);

            var package = requestDocument.Element("Package");
            var specialServices = package.Element("SpecialServices");
            Assert.That(specialServices, Is.Not.Null);
            Assert.That(specialServices.Element("SpecialService").Value, Is.EqualTo("8"));
        }

        [Test]
        public void DomesticCertificateOfMailingIsExpressed() {
            var requestDocument = UspsService.BuildDomesticShippingRequestDocument(
                "Joe User", 45.3, 1030.54M, "Big Box", 5, 3, 2, "98052", "90220",
                false, false, false, true, false);

            var package = requestDocument.Element("Package");
            var specialServices = package.Element("SpecialServices");
            Assert.That(specialServices, Is.Not.Null);
            Assert.That(specialServices.Element("SpecialService").Value, Is.EqualTo("9"));
        }

        [Test]
        public void DomesticElectronicConfirmationIsExpressed() {
            var requestDocument = UspsService.BuildDomesticShippingRequestDocument(
                "Joe User", 45.3, 1030.54M, "Big Box", 5, 3, 2, "98052", "90220",
                false, false, false, false, true);

            var package = requestDocument.Element("Package");
            var specialServices = package.Element("SpecialServices");
            Assert.That(specialServices, Is.Not.Null);
            Assert.That(specialServices.Element("SpecialService").Value, Is.EqualTo("16"));
        }

        [Test]
        public void DomesticServicesAreExpressed() {
            var requestDocument = UspsService.BuildDomesticShippingRequestDocument(
                "Joe User", 45.3, 1030.54M, "Big Box", 5, 3, 2, "98052", "90220",
                true, true, true, true, true);

            var package = requestDocument.Element("Package");
            var specialServices = package.Element("SpecialServices");
            Assert.That(specialServices, Is.Not.Null);
            var values = specialServices
                .Elements("SpecialService")
                .Select(el => el.Value)
                .ToList();
            Assert.That(values.Count, Is.EqualTo(5));
            Assert.That(values, Is.EquivalentTo(new[] {"4", "1", "8", "9", "16"}));
        }

        [Test]
        public void DomesticServicePricesReturnsDomesticPrices() {
            var uspsService = BuildFakeUspsService();
            var prices = uspsService.Prices(
                "Joe User", 45.3, 1030.54M, "Big Box", null, null,
                "United States", 5, 3, 2, "98052", "90220", false, false,
                false, false, false, false, false).ToList();
            Assert.That(prices.Count, Is.EqualTo(15));
            Assert.That(prices, new CollectionEquivalentConstraint(new[] {
                new ShippingOption {Description = "First-Class Mail", Price = 1.06M},
                new ShippingOption {Description = "Priority Mail<sup>®</sup>", Price = 24.85M},
                new ShippingOption {Description = "Priority Mail<sup>®</sup>", Price = 18.35M},
                new ShippingOption {Description = "Priority Mail<sup>®</sup> Large Flat Rate Box", Price = 14.85M},
                new ShippingOption {Description = "Priority Mail<sup>®</sup> Medium Flat Rate Box", Price = 12.35M},
                new ShippingOption {Description = "Priority Mail<sup>®</sup> Small Flat Rate Box", Price = 5.8M},
                new ShippingOption {Description = "Priority Mail<sup>®</sup> Flat Rate Envelope", Price = 5.6M},
                new ShippingOption {Description = "Priority Mail<sup>®</sup> Legal Flat Rate Envelope", Price = 5.75M},
                new ShippingOption {Description = "Priority Mail<sup>®</sup> Padded Flat Rate Envelope", Price = 5.95M},
                new ShippingOption {Description = "Priority Mail<sup>®</sup> Gift Card Flat Rate Envelope", Price = 5.6M},
                new ShippingOption {Description = "Priority Mail<sup>®</sup> Small Flat Rate Envelope", Price = 5.6M},
                new ShippingOption {Description = "Priority Mail<sup>®</sup> Window Flat Rate Envelope", Price = 5.6M},
                new ShippingOption {Description = "Standard Post<sup>®</sup>", Price = 18.35M},
                new ShippingOption {Description = "Media Mail<sup>®</sup>", Price = 6.52M},
                new ShippingOption {Description = "Library Mail", Price = 6.21M}
            }).Using(new ShippingOption.ShippingOptionComparer()));
        }

        [Test]
        public void DomesticServicePricesWithValidationExpressionSelectsMatchingMethods() {
            var uspsService = BuildFakeUspsService();
            var prices = uspsService.Prices(
                "Joe User", 45.3, 1030.54M, "Big Box", "Box", null,
                "United States", 5, 3, 2, "98052", "90220", false, false,
                false, false, false, false, false).ToList();
            Assert.That(prices.Count, Is.EqualTo(3));
            Assert.That(prices, new CollectionEquivalentConstraint(new[] {
                new ShippingOption {Description = "Priority Mail<sup>®</sup> Large Flat Rate Box", Price = 14.85M},
                new ShippingOption {Description = "Priority Mail<sup>®</sup> Medium Flat Rate Box", Price = 12.35M},
                new ShippingOption {Description = "Priority Mail<sup>®</sup> Small Flat Rate Box", Price = 5.8M},
            }).Using(new ShippingOption.ShippingOptionComparer()));
        }

        [Test]
        public void DomesticServicePricesWithExclusionExpressionExcludesMatchingMethods() {
            var uspsService = BuildFakeUspsService();
            var prices = uspsService.Prices(
                "Joe User", 45.3, 1030.54M, "Big Box", null, "Priority",
                "United States", 5, 3, 2, "98052", "90220", false, false,
                false, false, false, false, false).ToList();
            Assert.That(prices.Count, Is.EqualTo(4));
            Assert.That(prices, new CollectionEquivalentConstraint(new[] {
                new ShippingOption {Description = "First-Class Mail", Price = 1.06M},
                new ShippingOption {Description = "Standard Post<sup>®</sup>", Price = 18.35M},
                new ShippingOption {Description = "Media Mail<sup>®</sup>", Price = 6.52M},
                new ShippingOption {Description = "Library Mail", Price = 6.21M}
            }).Using(new ShippingOption.ShippingOptionComparer()));
        }

        [Test]
        public void DomesticServicePricesWithExclusionAndValidationExpressionsYieldsTheRightMethods() {
            var uspsService = BuildFakeUspsService();
            var prices = uspsService.Prices(
                "Joe User", 45.3, 1030.54M, "Big Box", "Mail", "Priority",
                "United States", 5, 3, 2, "98052", "90220", false, false,
                false, false, false, false, false).ToList();
            Assert.That(prices.Count, Is.EqualTo(3));
            Assert.That(prices, new CollectionEquivalentConstraint(new[] {
                new ShippingOption {Description = "First-Class Mail", Price = 1.06M},
                new ShippingOption {Description = "Media Mail<sup>®</sup>", Price = 6.52M},
                new ShippingOption {Description = "Library Mail", Price = 6.21M}
            }).Using(new ShippingOption.ShippingOptionComparer()));
        }

        [Test]
        public void DomesticServicePricesWithRegisteredMailYieldsBumpedPrice() {
            var uspsService = BuildFakeUspsService();
            var price = uspsService.Prices(
                "Joe User", 45.3, 1030.54M, "Big Box", null, null,
                "United States", 5, 3, 2, "98052", "90220", false, false,
                true, false, false, false, false).First();
            Assert.That(price.Price, Is.EqualTo(1.06 + 11.20));
            Assert.That(price.Description, Is.EqualTo("First-Class Mail"));
        }

        [Test]
        public void DomesticServicePricesWithInsuranceYieldsBumpedPrice() {
            var uspsService = BuildFakeUspsService();
            var price = uspsService.Prices(
                "Joe User", 45.3, 1030.54M, "Big Box", null, null,
                "United States", 5, 3, 2, "98052", "90220", false, false,
                false, true, false, false, false).First();
            Assert.That(price.Price, Is.EqualTo(1.06 + 1.95));
            Assert.That(price.Description, Is.EqualTo("First-Class Mail"));
        }

        [Test]
        public void DomesticServicePricesWithReturnReceiptYieldsBumpedPrice() {
            var uspsService = BuildFakeUspsService();
            var price = uspsService.Prices(
                "Joe User", 45.3, 1030.54M, "Big Box", null, null,
                "United States", 5, 3, 2, "98052", "90220", false, false,
                false, false, true, false, false).First();
            Assert.That(price.Price, Is.EqualTo(24.85M + 2.55M));
            Assert.That(price.Description, Is.EqualTo("Priority Mail<sup>®</sup>"));
        }

        [Test]
        public void DomesticServicePricesWithCertificateOfMailingYieldsBumpedPrice() {
            var uspsService = BuildFakeUspsService();
            var price = uspsService.Prices(
                "Joe User", 45.3, 1030.54M, "Big Box", null, null,
                "United States", 5, 3, 2, "98052", "90220", false, false,
                false, false, false, true, false).First();
            Assert.That(price.Price, Is.EqualTo(1.06 + 1.20));
            Assert.That(price.Description, Is.EqualTo("First-Class Mail"));
        }

        [Test]
        public void DomesticServicePricesWithConfirmationYieldsBumpedPrice() {
            var uspsService = BuildFakeUspsService();
            var price = uspsService.Prices(
                "Joe User", 45.3, 1030.54M, "Big Box", null, null,
                "United States", 5, 3, 2, "98052", "90220", false, false,
                false, false, false, false, true).First();
            Assert.That(price.Price, Is.EqualTo(24.85 + 1.25));
            Assert.That(price.Description, Is.EqualTo("Priority Mail<sup>®</sup>"));
        }

        [Test]
        public void DomesticServicePricesWithSeveralOptionsYieldsCombinedPrice() {
            var uspsService = BuildFakeUspsService();
            var price = uspsService.Prices(
                "Joe User", 45.3, 1030.54M, "Big Box", null, null,
                "United States", 5, 3, 2, "98052", "90220", false, false,
                true, true, false, true, false).First();
            Assert.That(price.Price, Is.EqualTo(1.06M + 11.20M + 1.95M + 1.20M));
            Assert.That(price.Description, Is.EqualTo("First-Class Mail"));
        }

        [Test]
        public void DomesticServicePricesWithUnavailableOptionsYieldsNothing() {
            var uspsService = BuildFakeUspsService();
            var prices = uspsService.Prices(
                "Joe User", 45.3, 1030.54M, "Big Box", null, null,
                "United States", 5, 3, 2, "98052", "90220", false, false,
                true, true, true, true, false);
            Assert.That(prices, Is.Empty);
        }
    }
}