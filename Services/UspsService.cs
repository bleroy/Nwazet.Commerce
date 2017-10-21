using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using System.Xml.XPath;
using Nwazet.Commerce.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using System.Globalization;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Usps.Shipping")]
    public class UspsService : IUspsService {
        private readonly IUspsWebService _webService;
        private readonly IWorkContextAccessor _wca;
        private readonly IEnumerable<IShippingAreaProvider> _shippingAreaProviders;

        private const string Us = "us";
        private static readonly IEnumerable<ShippingOption> Nothing = new ShippingOption[0];

        public const int RegisteredMailDomestic = 4;
        public const int InsuranceDomestic = 1;
        public const int ReturnReceiptDomestic = 8;
        public const int CertificateOfMailingDomestic = 9;
        public const int ElectronicConfirmationDomestic = 16;

        public const int RegisteredMailInternational = 0;
        public const int InsuranceInternational = 1;
        public const int ReturnReceiptInternational = 2;
        public const int CertificateOfMailingInternational = 6;
        public const int ElectronicConfirmationInternational = 9;

        public UspsService(
            IUspsWebService webService,
            IWorkContextAccessor wca,
            IEnumerable<IShippingAreaProvider> shippingAreaProviders) {

            _webService = webService;
            _wca = wca;
            _shippingAreaProviders = shippingAreaProviders;
        }

        public IEnumerable<string> GetInternationalShippingAreas() {
            var shippingAreas = _shippingAreaProviders
                .SelectMany(p => p.GetAreas());
            return shippingAreas.Where(a => a.Id != Us).Select(a => a.Id).ToList();
        }

        public IEnumerable<string> GetDomesticShippingAreas() {
            yield return Us;
        }

        public UspsSettingsPart GetSettings() {
            var workContext = _wca.GetContext();
            return workContext.CurrentSite.As<UspsSettingsPart>();
        }

        public static XElement BuildDomesticShippingRequestDocument(
            string userId,
            double weightInOunces,
            decimal valueOfContents,
            string container,
            int lengthInInches,
            int widthInInches,
            int heightInInches,
            string originZip,
            string destinationZip,
            bool registeredMail = false,
            bool insurance = false,
            bool returnReceipt = false,
            bool certificateOfMailing = false,
            bool electronicConfirmation = false
            ) {

            var pounds = OuncesToPounds(weightInOunces);
            var ounces = RemainderOunces(weightInOunces, pounds);
            var large = UspsContainer.List.ContainsKey(container)
                            ? UspsContainer.List[container].Size == UspsContainer.ContainerSize.Large
                            : IsLarge(lengthInInches, widthInInches, heightInInches);
            // Machinable parcels defined in http://pe.usps.com/text/dmm300/101.htm
            var machinable = IsMachinable(weightInOunces, lengthInInches, widthInInches, heightInInches);

            var extras = new XElement("SpecialServices");
            if (registeredMail) {
                AddSpecialService(extras, RegisteredMailDomestic);
            }
            if (insurance) {
                AddSpecialService(extras, InsuranceDomestic);
            }
            if (returnReceipt) {
                AddSpecialService(extras, ReturnReceiptDomestic);
            }
            if (certificateOfMailing) {
                AddSpecialService(extras, CertificateOfMailingDomestic);
            }
            if (electronicConfirmation) {
                AddSpecialService(extras, ElectronicConfirmationDomestic);
            }

            var packageElement = new XElement("Package",
                                              new XAttribute("ID", "0"),
                                              new XElement("Service", "ALL"),
                                              new XElement("FirstClassMailType", "PARCEL"),
                                              new XElement("ZipOrigination", originZip),
                                              new XElement("ZipDestination", destinationZip),
                                              new XElement("Pounds", pounds),
                                              new XElement("Ounces", ounces.ToString("G2")),
                                              new XElement("Container", container.ToUpperInvariant()),
                                              new XElement("Size", large ? "LARGE" : "REGULAR"),
                                              new XElement("Width", widthInInches),
                                              new XElement("Length", lengthInInches),
                                              new XElement("Height", heightInInches),
                                              new XElement("Value", valueOfContents.ToString("F2")),
                                              new XElement("SortBy", "PACKAGE"),
                                              new XElement("Machinable", machinable)
                );
            if (extras.HasElements) {
                var valueElement = packageElement.Element("Value");
                if (valueElement != null) valueElement.AddAfterSelf(extras);
            }

            return new XElement("RateV4Request",
                                new XAttribute("USERID", userId),
                                new XElement("Revision", 2),
                                packageElement);
        }

        private static void AddSpecialService(XElement extras, int serviceId) {
            extras.Add(new XElement("SpecialService", serviceId));
        }

        public static XElement BuildInternationalShippingRequestDocument(
            string userId,
            double weightInOunces,
            decimal valueOfContents,
            string container,
            string country,
            int lengthInInches,
            int widthInInches,
            int heightInInches,
            string originZip,
            bool commercialPrices = false,
            bool commercialPlusPrices = false,
            bool registeredMail = false,
            bool insurance = false,
            bool returnReceipt = false,
            bool certificateOfMailing = false,
            bool electronicConfirmation = false
            ) {

            var pounds = OuncesToPounds(weightInOunces);
            var ounces = RemainderOunces(weightInOunces, pounds);
            var large = IsLarge(lengthInInches, widthInInches, heightInInches);
            var machinable = IsMachinable(weightInOunces, lengthInInches, widthInInches, heightInInches);

            var extras = new XElement("ExtraServices");
            if (registeredMail) {
                AddExtraService(extras, RegisteredMailInternational);
            }
            if (insurance) {
                AddExtraService(extras, InsuranceInternational);
            }
            if (returnReceipt) {
                AddExtraService(extras, ReturnReceiptInternational);
            }
            if (certificateOfMailing) {
                AddExtraService(extras, CertificateOfMailingInternational);
            }
            if (electronicConfirmation) {
                AddExtraService(extras, ElectronicConfirmationInternational);
            }

            var package = new XElement("Package", new XAttribute("ID", "0"),
                                       new XElement("Pounds", pounds),
                                       new XElement("Ounces", ounces.ToString("G2")),
                                       new XElement("Machinable", machinable),
                                       new XElement("MailType", "Package"),
                                       new XElement("ValueOfContents", valueOfContents.ToString("F2", CultureInfo.GetCultureInfo("en-US"))), //assume dollars
                                       new XElement("Country", country),
                                       new XElement("Container", container),
                                       new XElement("Size", large ? "LARGE" : "REGULAR"),
                                       new XElement("Width", widthInInches),
                                       new XElement("Length", lengthInInches),
                                       new XElement("Height", heightInInches),
                                       new XElement("Girth", 0),
                                       new XElement("OriginZip", originZip),
                                       new XElement("CommercialFlag", commercialPrices ? "Y" : "N"),
                                       new XElement("CommercialPlusFlag", commercialPlusPrices ? "Y" : "N")
                );
            if (extras.HasElements) {
                package.Add(extras);
            }

            return new XElement("IntlRateV2Request",
                                new XAttribute("USERID", userId),
                                new XElement("Revision", 2),
                                package);
        }

        private static int OuncesToPounds(double weightInOunces) {
            return (int) Math.Truncate(weightInOunces/16);
        }

        private static double RemainderOunces(double weightInOunces, int pounds) {
            return Math.Ceiling(weightInOunces - 16 * pounds);
        }

        private static void AddExtraService(XElement extras, int serviceId) {
            extras.Add(new XElement("ExtraService", serviceId));
        }

        private static bool IsLarge(int lengthInInches, int widthInInches, int heightInInches) {
            // more than 12" on any dimension is large
            return lengthInInches > 12 || widthInInches > 12 || heightInInches > 12;
        }

        private static bool IsMachinable(double weightInOunces, int lengthInInches, int widthInInches,
                                         int heightInInches) {
            return lengthInInches >= 6 && heightInInches >= 3 && widthInInches >= 1
                   && lengthInInches <= 27 && heightInInches <= 17 && widthInInches <= 17
                   && weightInOunces >= 6 && weightInOunces <= 400;
        }

        public IEnumerable<ShippingOption> Prices(
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
            ) {

            var isDomestic = country == Country.UnitedStates;
            var requestDocument = isDomestic
                                      ? BuildDomesticShippingRequestDocument(userId, weightInOunces, valueOfContents,
                                                                             container,
                                                                             lengthInInches, widthInInches,
                                                                             heightInInches,
                                                                             originZip, destinationZip, registeredMail,
                                                                             insurance, returnReceipt,
                                                                             certificateOfMailing,
                                                                             electronicConfirmation)
                                      : BuildInternationalShippingRequestDocument(userId, weightInOunces,
                                                                                  valueOfContents, container,
                                                                                  country, lengthInInches, widthInInches,
                                                                                  heightInInches, originZip,
                                                                                  commercialPrices, commercialPlusPrices,
                                                                                  registeredMail, insurance,
                                                                                  returnReceipt,
                                                                                  certificateOfMailing,
                                                                                  electronicConfirmation);
            var url = isDomestic
                          ? BuildDomesticShippingUrl(requestDocument)
                          : BuildInternationalShippingUrl(requestDocument);
            var responseDocument = _webService.QueryWebService(url);
            ThrowOnErrorResponse(responseDocument);

            var serviceNameValidation = String.IsNullOrEmpty(serviceNameValidationExpression)
                                            ? null
                                            : new Regex(serviceNameValidationExpression, RegexOptions.IgnoreCase);
            var serviceNameExclusion = String.IsNullOrEmpty(serviceNameExclusionExpression)
                                           ? null
                                           : new Regex(serviceNameExclusionExpression, RegexOptions.IgnoreCase);

            if (isDomestic) {
                return responseDocument
                    .Descendants("Postage")
                    .SelectMany(s => {
                        var descriptionBuilder = new List<string>();
                        var descriptionNode = s.Element("MailService");
                        var descriptionNodeValue =
                            descriptionNode == null
                                ? null
                                : HttpUtility.HtmlDecode(descriptionNode.Value);
                        if (descriptionNodeValue != null && serviceNameValidation != null &&
                            !serviceNameValidation.IsMatch(descriptionNodeValue)) {
                            return Nothing;
                        }
                        if (descriptionNodeValue != null && serviceNameExclusion != null &&
                            serviceNameExclusion.IsMatch(descriptionNodeValue)) {
                            return Nothing;
                        }
                        if (descriptionNodeValue != null) {
                            descriptionBuilder.Add(descriptionNodeValue);
                        }
                        var basePriceNode = commercialPlusPrices
                                                ? s.Element("CommercialPlusRate")
                                                : commercialPrices
                                                      ? s.Element("CommercialRate")
                                                      : s.Element("Rate");
                        if (basePriceNode == null) {
                            throw new InvalidOperationException("USPS rate not found in response");
                        }
                        var price = decimal.Parse(basePriceNode.Value, CultureInfo.GetCultureInfo("en-US")); //US web service
                        if (registeredMail) {
                            var serviceNode = FindDomesticServiceNode(s, RegisteredMailDomestic);
                            if (serviceNode == null) return Nothing;
                            price += decimal.Parse(serviceNode.Value, CultureInfo.GetCultureInfo("en-US"));
                            AddServiceNameToDescription(serviceNode, descriptionBuilder);
                        }
                        if (insurance) {
                            var serviceNode = FindDomesticServiceNode(s, InsuranceDomestic);
                            if (serviceNode == null) return Nothing;
                            price += decimal.Parse(serviceNode.Value, CultureInfo.GetCultureInfo("en-US"));
                            AddServiceNameToDescription(serviceNode, descriptionBuilder);
                        }
                        if (returnReceipt) {
                            var serviceNode = FindDomesticServiceNode(s, ReturnReceiptDomestic);
                            if (serviceNode == null) return Nothing;
                            price += decimal.Parse(serviceNode.Value, CultureInfo.GetCultureInfo("en-US"));
                            AddServiceNameToDescription(serviceNode, descriptionBuilder);
                        }
                        if (certificateOfMailing) {
                            var serviceNode = FindDomesticServiceNode(s, CertificateOfMailingDomestic);
                            if (serviceNode == null) return Nothing;
                            price += decimal.Parse(serviceNode.Value, CultureInfo.GetCultureInfo("en-US"));
                            AddServiceNameToDescription(serviceNode, descriptionBuilder);
                        }
                        if (electronicConfirmation) {
                            var serviceNode = FindDomesticServiceNode(s, ElectronicConfirmationDomestic);
                            if (serviceNode == null) return Nothing;
                            price += decimal.Parse(serviceNode.Value, CultureInfo.GetCultureInfo("en-US"));
                            AddServiceNameToDescription(serviceNode, descriptionBuilder);
                        }
                        return new[] {
                            new ShippingOption {
                                Description = string.Join(" - ", descriptionBuilder),
                                Price = price,
                                IncludedShippingAreas = GetDomesticShippingAreas().ToList(),
                                ExcludedShippingAreas = GetInternationalShippingAreas().ToList()
                            }
                        };
                    });
            }
            return responseDocument
                .Descendants("Service")
                .SelectMany(s => {
                    var descriptionBuilder = new List<string>();
                    var descriptionNode = s.Element("SvcDescription");
                    var descriptionNodeValue =
                        descriptionNode == null
                            ? null
                            : HttpUtility.HtmlDecode(descriptionNode.Value);
                    if (descriptionNodeValue != null && serviceNameValidation != null &&
                        !serviceNameValidation.IsMatch(descriptionNodeValue)) {
                        return Nothing;
                    }
                    if (descriptionNodeValue != null && serviceNameExclusion != null &&
                        serviceNameExclusion.IsMatch(descriptionNodeValue)) {
                        return Nothing;
                    }
                    if (descriptionNodeValue != null) descriptionBuilder.Add(descriptionNodeValue);
                    
                    var basePriceNode = commercialPlusPrices
                                            ? s.Element("CommercialPlusPostage")
                                            : commercialPrices ? s.Element("CommercialPostage") : s.Element("Postage");
                    if (basePriceNode == null) {
                        throw new InvalidOperationException("USPS rate not found in response");
                    }
                    var price = decimal.Parse(basePriceNode.Value, CultureInfo.GetCultureInfo("en-US")); //US webservice
                    if (registeredMail) {
                        var serviceNode = FindInternationalServiceNode(s, RegisteredMailInternational);
                        if (serviceNode == null) return Nothing;
                        price += decimal.Parse(serviceNode.Value, CultureInfo.GetCultureInfo("en-US"));
                        AddServiceNameToDescription(serviceNode, descriptionBuilder);
                    }
                    if (insurance) {
                        var serviceNode = FindInternationalServiceNode(s, InsuranceInternational);
                        if (serviceNode == null) return Nothing;
                        price += decimal.Parse(serviceNode.Value, CultureInfo.GetCultureInfo("en-US"));
                        AddServiceNameToDescription(serviceNode, descriptionBuilder);
                    }
                    if (returnReceipt) {
                        var serviceNode = FindInternationalServiceNode(s, ReturnReceiptInternational);
                        if (serviceNode == null) return Nothing;
                        price += decimal.Parse(serviceNode.Value, CultureInfo.GetCultureInfo("en-US"));
                        AddServiceNameToDescription(serviceNode, descriptionBuilder);
                    }
                    if (certificateOfMailing) {
                        var serviceNode = FindInternationalServiceNode(s, CertificateOfMailingInternational);
                        if (serviceNode == null) return Nothing;
                        price += decimal.Parse(serviceNode.Value, CultureInfo.GetCultureInfo("en-US"));
                        AddServiceNameToDescription(serviceNode, descriptionBuilder);
                    }
                    if (electronicConfirmation) {
                        var serviceNode = FindInternationalServiceNode(s, ElectronicConfirmationInternational);
                        if (serviceNode == null) return Nothing;
                        price += decimal.Parse(serviceNode.Value, CultureInfo.GetCultureInfo("en-US"));
                        AddServiceNameToDescription(serviceNode, descriptionBuilder);
                    }

                    var commitmentNode = s.Element("SvcCommitments");
                    if (commitmentNode != null)
                    {
                        descriptionBuilder.Add(commitmentNode.Value);
                    }

                    return new[] {
                        new ShippingOption {
                            Description = string.Join("; ", descriptionBuilder),
                            Price = price,
                            IncludedShippingAreas = GetInternationalShippingAreas().ToList(),
                            ExcludedShippingAreas = GetDomesticShippingAreas().ToList()
                        }
                    };
                });
        }

        private static void AddServiceNameToDescription(XElement serviceNode, List<string> descriptionBuilder) {
            var serviceNameNode = serviceNode.Element("ServiceName");
            if (serviceNameNode != null) descriptionBuilder.Add(serviceNameNode.Value);
        }

        private Uri BuildDomesticShippingUrl(XElement requestDocument) {
            var serviceUri = GetServiceUri();
            var queryString = HttpUtility.ParseQueryString("API=RateV4");
            queryString.Add("XML", requestDocument.ToString(SaveOptions.DisableFormatting));
            serviceUri.Query = queryString.ToString();
            return serviceUri.Uri;
        }

        private Uri BuildInternationalShippingUrl(XElement requestDocument) {
            var serviceUri = GetServiceUri();
            var queryString = HttpUtility.ParseQueryString("API=IntlRateV2");
            queryString.Add("XML", requestDocument.ToString(SaveOptions.DisableFormatting));
            serviceUri.Query = queryString.ToString();
            return serviceUri.Uri;
        }

        private static UriBuilder GetServiceUri(bool production = true) {
            var serviceUri = new UriBuilder("http", "production.shippingapis.com", 80,
                                            production ? "ShippingAPI.dll" : "ShippingAPITest.dll");
            return serviceUri;
        }

        private static void ThrowOnErrorResponse(XElement responseDocument) {
            if (responseDocument.DescendantsAndSelf("Error").Any()) {
                var errorDescription = responseDocument.DescendantsAndSelf("Error").First().Element("Description");
                if (errorDescription != null) throw new ShippingException(errorDescription.Value);
                throw new ShippingException();
            }
        }

        private static XElement FindInternationalServiceNode(XElement s, int serviceId) {
            var serviceNode =
                s.XPathSelectElement(
                    "ExtraServices/ExtraService[ServiceID=\"" + serviceId + "\" and Available=\"True\"]/Price");
            return serviceNode;
        }

        private static XElement FindDomesticServiceNode(XElement s, int serviceId) {
            var serviceNode =
                s.XPathSelectElement(
                    "SpecialServices/SpecialService[ServiceID=\"" + serviceId + "\" and Available=\"true\"]/Price");
            return serviceNode;
        }
    }
}
