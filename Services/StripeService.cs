using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web.Security;
using Newtonsoft.Json.Linq;
using Nwazet.Commerce.Exceptions;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.ViewModels;
using Orchard.Caching;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Stripe")]
    public class StripeService : IStripeService {
        public const string CryptoPurpose = "strip checkout";

        private readonly IStripeWebService _webService;
        private readonly IWorkContextAccessor _wca;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;
        private readonly dynamic _shapeFactory;

        public StripeService(
            IStripeWebService webService,
            IWorkContextAccessor wca,
            ICacheManager cacheManager,
            ISignals signals,
            IShapeFactory shapeFactory) {

            _webService = webService;
            _wca = wca;
            _cacheManager = cacheManager;
            _signals = signals;
            _shapeFactory = shapeFactory;
        }

        public string Name {get { return "Stripe"; }}

        public StripeSettingsPart GetSettings() {
            return _cacheManager.Get(
                "StripeSettings",
                ctx => {
                    ctx.Monitor(_signals.When("Stripe.Changed"));
                    var workContext = _wca.GetContext();
                    return (StripeSettingsPart) workContext
                        .CurrentSite
                        .ContentItem
                        .Get(typeof (StripeSettingsPart));
                });
        }

        public dynamic BuildCheckoutButtonShape(
            IEnumerable<dynamic> productShapes,
            IEnumerable<ShoppingCartQuantityProduct> productQuantities,
            IEnumerable<ShippingOption> shippingOptions,
            TaxAmount taxes,
            string country,
            string zipCode,
            IEnumerable<string> custom) {

            var checkoutSettings = GetSettings();
            var shipping = shippingOptions.ToList();
            var shippingOption = shipping[0];
            var productShapeList = productShapes.ToList();

            var data = new Tuple<CheckoutItem[], double, string, string, TaxAmount, string, string>(
                productShapeList.Select(p => new CheckoutItem {
                    ProductId = p.Product.Id,
                    Quantity = p.Quantity,
                    Price = p.DiscountedPrice,
                    LinePriceAdjustment = p.LinePriceAdjustment,
                    Title = p.Title
                            + (p.ProductAttributes == null
                                ? ""
                                : " (" + string.Join(", ", p.ProductAttributes.Values) + ")")
                }).ToArray(),
                shippingOption.Price, shippingOption.Description, shippingOption.ShippingCompany,
                taxes,
                country,
                zipCode);
            var formatter = new BinaryFormatter();
            string encryptedData;
            using (var stream = new MemoryStream()) {
                formatter.Serialize(stream, data);
                encryptedData = Convert.ToBase64String(MachineKey.Protect(stream.ToArray(), CryptoPurpose));
            }

            return _shapeFactory.Stripe(
                EncryptedData: encryptedData,
                CartItems: productShapeList,
                ShippingOptions: shipping,
                Taxes: taxes,
                Country: country,
                ZipCode: zipCode,
                Custom: custom,
                PublishableKey: checkoutSettings.PublishableKey);
        }

        public StripeCheckoutViewModel DecryptCheckoutData(string checkoutData) {
            var binaryEncryptedData = Convert.FromBase64String(checkoutData);
            var decryptedData = MachineKey.Unprotect(binaryEncryptedData, CryptoPurpose);
            if (decryptedData == null) return null;
            var formatter = new BinaryFormatter();
            Tuple<CheckoutItem[], double, string, string, TaxAmount, string, string> data;
            using (var stream = new MemoryStream(decryptedData)) {
                data = (Tuple<CheckoutItem[], double, string, string, TaxAmount, string, string>)
                    formatter.Deserialize(stream);
            }
            var cartItems = data.Item1;
            var shippingOption = new ShippingOption {
                Price = data.Item2,
                Description = data.Item3,
                ShippingCompany = data.Item4
            };
            var taxes = data.Item5;
            var country = data.Item6;
            var zipCode = data.Item7;
            var stripeData = new StripeCheckoutViewModel {
                BillingAddress = new Address {
                    Country = country,
                    PostalCode = zipCode
                },
                ShippingAddress = new Address {
                    Country = country,
                    PostalCode = zipCode
                },
                CheckoutItems = cartItems,
                ShippingOption = shippingOption,
                Taxes = taxes
            };
            return stripeData;
        }

        public CreditCardCharge Charge(string token, double amount) {
            var settings = GetSettings();
            JObject chargeResult;
            try {
                chargeResult = _webService.Query(
                    settings.SecretKey, "charges",
                    new NameValueCollection {
                        {"amount", (amount*100).ToString("F0")},
                        {"currency", settings.Currency},
                        {"card", token},
                        {"description", _wca.GetContext().CurrentSite.SiteName}
                    });
            }
            catch (StripeException ex) {
                if (ex.Response == null) throw;
                var exceptionResult = ex.Response["error"];
                return new CreditCardCharge {
                    Error = new CheckoutError {
                        Type = exceptionResult.Value<string>("type"),
                        Message = exceptionResult.Value<string>("message"),
                        Code = exceptionResult.Value<string>("code")
                    }
                };
            }
            var card = chargeResult["card"];
            return new CreditCardCharge {
                TransactionId = Name + ":" + chargeResult.Value<string>("id"),
                Last4 = card.Value<string>("last4"),
                ExpirationMonth = card.Value<int>("exp_month"),
                ExpirationYear = card.Value<int>("exp_year")
            };
        }

        public bool IsInTestMode() {
            return GetSettings().PublishableKey.StartsWith("pk_test_");
        }

        public string GetChargeAdminUrl(string transactionId) {
            if (!transactionId.StartsWith(Name + ":")) return null;

            const string urlTestPattern = "https://manage.stripe.com/test/payments/{0}";
            const string urlPattern = "https://manage.stripe.com/payments/{0}";

            return string.Format(
                IsInTestMode() ? urlTestPattern : urlPattern,
                transactionId.Substring(Name.Length + 1));
        }
    }
}