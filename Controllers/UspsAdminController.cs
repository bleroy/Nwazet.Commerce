using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Xml.Linq;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.UI.Admin;

namespace Nwazet.Commerce.Controllers {
    [Admin]
    [OrchardFeature("Usps.Shipping")]
    public class UspsAdminController : Controller {
        private readonly IUspsService _uspsService;

        public UspsAdminController(IUspsService uspsService) {
            _uspsService = uspsService;
        }

        [HttpPost]
        [OutputCache(Duration = 0)]
        public ActionResult Price(
            string userId,
            double weightInOunces,
            double paddingWeight,
            double valueOfContents,
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

            try {
                var prices = _uspsService.Prices(
                    userId, weightInOunces + paddingWeight, valueOfContents, container,
                    serviceNameValidationExpression, serviceNameExclusionExpression,
                    country,
                    lengthInInches, widthInInches, heightInInches, originZip,
                    destinationZip,
                    commercialPrices, commercialPlusPrices, registeredMail, insurance,
                    returnReceipt,
                    certificateOfMailing, electronicConfirmation);
                return Json(prices);
            }
            catch (ShippingException ex) {
                return Json(new {
                    error = ex.Message
                });
            }
        }

        [HttpPost]
        [OutputCache(Duration = 0)]
        public ActionResult PerformTestRequest1() {
            return DoVerifyQuery("6406 Ivy Lane", "Greenbelt", "MD", "");
        }

        [HttpPost]
        [OutputCache(Duration = 0)]
        public ActionResult PerformTestRequest2() {
            return DoVerifyQuery("8 Wildwood Drive", "Old Lyme", "CT", "06371");
        }

        [HttpPost]
        [OutputCache(Duration = 0)]
        public ActionResult PerformTestRequest3() {
            return DoCityStateLookupQuery("90210");
        }

        [HttpPost]
        [OutputCache(Duration = 0)]
        public ActionResult PerformTestRequest4() {
            return DoCityStateLookupQuery("20770");
        }

        [HttpPost]
        [OutputCache(Duration = 0)]
        public ActionResult PerformTestRequest5() {
            return DoZipCodeLookupQuery("6406 Ivy Lane", "Greenbelt", "MD");
        }

        [HttpPost]
        [OutputCache(Duration = 0)]
        public ActionResult PerformTestRequest6() {
            return DoZipCodeLookupQuery("8 Wildwood Drive", "Old Lyme", "CT");
        }

        [HttpPost]
        [OutputCache(Duration = 0)]
        public ActionResult PerformTestRequest7() {
            return DoTrackQuery("EJ958083578US");
        }

        [HttpPost]
        [OutputCache(Duration = 0)]
        public ActionResult PerformTestRequest8() {
            return DoTrackQuery("EJ958088694US");
        }

        [HttpPost]
        [OutputCache(Duration = 0)]
        public ActionResult PerformTestRequest9() {
            return DoExpressMailCommitmentQuery("207", "11210");
        }

        [HttpPost]
        [OutputCache(Duration = 0)]
        public ActionResult PerformTestRequest10() {
            return DoExpressMailCommitmentQuery("20770", "11210", new DateTime(2004, 8, 5));
        }

        [HttpPost]
        [OutputCache(Duration = 0)]
        public ActionResult PerformTestRequest11() {
            return DoPickupAvailabilityQuery("ABC Corp.", "Suite 777", "1390 Market Street", "Houston", "TX", "77058",
                                             "1234");
        }

        [HttpPost]
        [OutputCache(Duration = 0)]
        public ActionResult PerformTestRequest12()
        {
            return DoPickupAvailabilityQuery("", "", "1390 Market Street", "", "", "77058", "");
        }

        private ActionResult DoVerifyQuery(string address, string city, string state, string zip) {
            const string api = "Verify";
            const string requestTag = "AddressValidateRequest";
            var query = "<Address ID=\"0\"><Address1></Address1><Address2>" + address + "</Address2><City>" + city +
                        "</City><State>" + state + "</State><Zip5>" + zip + "</Zip5><Zip4></Zip4></Address>";

            var responseDocument = ResponseDocument(api, requestTag, query);
            var error = CheckForError(responseDocument);
            if (error != null) return error;
            var addressNode = responseDocument.Element("Address");
            if (addressNode != null) {
                return Json(new {
                    Address = addressNode.El("Address2"),
                    City = addressNode.El("City"),
                    State = addressNode.El("State"),
                    Zip = addressNode.El("Zip5") + "-" + addressNode.El("Zip4")
                });
            }
            return Json(new {error = "Couldn't parse USPS response."});
        }

        private ActionResult DoCityStateLookupQuery(string zip) {
            const string api = "CityStateLookup";
            const string requestTag = "CityStateLookupRequest";
            var query = "<ZipCode ID=\"0\"><Zip5>" + zip + "</Zip5></ZipCode>";

            var responseDocument = ResponseDocument(api, requestTag, query);
            var error = CheckForError(responseDocument);
            if (error != null) return error;
            var zipNode = responseDocument.Element("ZipCode");
            if (zipNode != null) {
                return Json(new {
                    City = zipNode.El("City"),
                    State = zipNode.El("State"),
                    Zip = zipNode.El("Zip5")
                });
            }
            return Json(new {error = "Couldn't parse USPS response."});
        }

        private ActionResult DoZipCodeLookupQuery(string address, string city, string state) {
            const string api = "ZipCodeLookup";
            const string requestTag = "ZipCodeLookupRequest";
            var query = "<Address ID=\"0\"><Address1></Address1><Address2>" + address + "</Address2><City>" + city +
                        "</City><State>" + state + "</State></Address>";

            var responseDocument = ResponseDocument(api, requestTag, query);
            var error = CheckForError(responseDocument);
            if (error != null) return error;
            var addressNode = responseDocument.Element("Address");
            if (addressNode != null) {
                return Json(new {
                    Address = addressNode.El("Address2"),
                    City = addressNode.El("City"),
                    State = addressNode.El("State"),
                    Zip = addressNode.El("Zip5") + "-" + addressNode.El("Zip4")
                });
            }
            return Json(new {error = "Couldn't parse USPS response."});
        }

        private ActionResult DoTrackQuery(string trackId) {
            const string api = "TrackV2";
            const string requestTag = "TrackRequest";
            string query = "<TrackID ID=\"" + trackId + "\"></TrackID>";

            var responseDocument = ResponseDocument(api, requestTag, query);
            var error = CheckForError(responseDocument);
            if (error != null) return error;
            var trackNode = responseDocument.Element("TrackInfo");
            if (trackNode != null) {
                return Json(new {
                    Summary = trackNode.El("TrackSummary"),
                    Details = trackNode.Elements("TrackDetail")
                                       .Select(el => el.Value)
                });
            }
            return Json(new {error = "Couldn't parse USPS response."});
        }

        private ActionResult DoExpressMailCommitmentQuery(string originZip, string destinationZip, DateTime? date = null) {
            const string api = "ExpressMailCommitment";
            const string requestTag = "ExpressMailCommitmentRequest";
            var dateString = date == null ? "" : date.Value.ToString("dd-MMM-yyyy");
            var query = "<OriginZIP>" + originZip + "</OriginZIP><DestinationZIP>" + destinationZip +
                        "</DestinationZIP><Date>" + dateString + "</Date>";

            var responseDocument = ResponseDocument(api, requestTag, query);
            var error = CheckForError(responseDocument);
            if (error != null) return error;
            var commitmentNodes = responseDocument.Elements("Commitment").ToList();
            if (commitmentNodes.Any()) {
                return Json(new {
                    OriginZip = responseDocument.El("OriginZIP"),
                    OriginCity = responseDocument.El("OriginCity"),
                    OriginState = responseDocument.El("OriginState"),
                    DestinationZip = responseDocument.El("DestinationZIP"),
                    DestinationCity = responseDocument.El("DestinationCity"),
                    DestinationState = responseDocument.El("DestinationState"),
                    Date = responseDocument.El("Date"),
                    Time = responseDocument.El("Time"),
                    Commitments = commitmentNodes.Select(c => new {
                        CommitmentName = c.El("CommitmentName"),
                        CommitmentTime = c.El("CommitmentTime"),
                        CommitmentSequence = c.El("CommitmentSequence"),
                        Locations = c.Elements("Location").Select(l => new {
                            CutOff = l.El("CutOff"),
                            Facility = l.El("Facility"),
                            Street = l.El("Street"),
                            City = l.El("City"),
                            State = l.El("State"),
                            CutOZipff = l.El("Zip")
                        })
                    })
                });
            }
            return Json(new {error = "Couldn't parse USPS response."});
        }

        private ActionResult DoPickupAvailabilityQuery(string firm, string suiteOrApt, string address, string city,
                                                       string state, string zip5, string zip4) {
            const string api = "CarrierPickupAvailability";
            const string requestTag = "CarrierPickupAvailabilityRequest";
            var query = "<FirmName>" + firm + "</FirmName><SuiteOrApt>" + suiteOrApt + "</SuiteOrApt><Address2>" +
                        address + "</Address2><Urbanization></Urbanization><City>" + city + "</City><State>" + state +
                        "</State><ZIP5>" + zip5 + "</ZIP5><ZIP4>" + zip4 + "</ZIP4>";

            var responseDocument = ResponseDocument(api, requestTag, query, true);
            var error = CheckForError(responseDocument);
            if (error != null) return error;
            return Json(new {
                FirmName = responseDocument.El("FirmName"),
                SuiteOrApt = responseDocument.El("SuiteOrApt"),
                Address = responseDocument.El("Address2"),
                City = responseDocument.El("City"),
                State = responseDocument.El("State"),
                Zip = responseDocument.El("ZIP5") + "-" + responseDocument.El("ZIP4"),
                DayOfWeek = responseDocument.El("DayOfWeek"),
                Date = responseDocument.El("Date"),
                CarrierRoute = responseDocument.El("CarrierRoute")
            });
        }

        private XElement ResponseDocument(string api, string requestTag, string query, bool ssl = false) {
            var settings = _uspsService.GetSettings();
            var url = (ssl
                           ? "https://secure"
                           : "http://production") +
                      ".shippingapis.com/ShippingAPITest.dll?API=" + api + "&XML=<" + requestTag +
                      " USERID=\"" + settings.UserId + "\">" + query + "</" + requestTag + ">";
            var request = new WebClient();
            var response = request.DownloadString(url);
            var responseDocument = XElement.Parse(response);
            return responseDocument;
        }

        private ActionResult CheckForError(XElement responseDocument) {
            if (responseDocument.Name != "Error") return null;
            var errorDescription = responseDocument.Element("Description");
            return Json(errorDescription != null ? new {error = errorDescription.Value} : new {error = "USPS error"});
        }
    }
}