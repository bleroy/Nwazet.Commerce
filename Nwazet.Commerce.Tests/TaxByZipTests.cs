using System.Collections.Generic;
using NUnit.Framework;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.Tests.Helpers;
using System;
using System.Linq;

namespace Nwazet.Commerce.Tests
{
    class TaxByZipTests
    {
        private string CsvRates {
            get {
                return "52411," + (.07M).ToString() +
                    "\r\n52627," + (.05M).ToString() + 
                    "\r\n52405," + (.1M).ToString() + 
                    "\r\n52412," + (.08M).ToString();
            }
        }
        private string TabRates {
            get {
                return "52411\t" + (.07M).ToString() + 
                    "\n52627\t" + (.05M).ToString() + 
                    "\n52405\t" + (.1M).ToString() + 
                    "\n52412\t" + (.08M).ToString();
            }
        }

        [Test]
        public void CsvRatesAreParsedCorrectly() {
            var csvZipTax = new ZipCodeTaxPart();
            ContentHelpers.PreparePart(csvZipTax, "Tax");
            csvZipTax.Rates = CsvRates;

            var Rates = csvZipTax.GetRates();
            var rKeys = Rates.Keys.ToArray();
            var rValues = Rates.Values.ToArray();
            Assert.That(Rates.Count, Is.EqualTo(4));
            Assert.That(rKeys[0], Is.EqualTo("52411"));
            Assert.That(rValues[0], Is.EqualTo(.07M));
            Assert.That(rKeys[1], Is.EqualTo("52627"));
            Assert.That(rValues[1], Is.EqualTo(.05M));
            Assert.That(rKeys[2], Is.EqualTo("52405"));
            Assert.That(rValues[2], Is.EqualTo(.1M));
            Assert.That(rKeys[3], Is.EqualTo("52412"));
            Assert.That(rValues[3], Is.EqualTo(.08M));
        }

        [Test]
        public void RightTaxAppliesToCsvRates() {
            var csvZipTax = new ZipCodeTaxPart();
            ContentHelpers.PreparePart(csvZipTax, "Tax");
            csvZipTax.Rates = CsvRates;
            
            var taxProvider = new FakeTaxProvider(new[] { csvZipTax });
            var taxHelper = new ZipCodeTaxComputationHelper();
            var cart = ShoppingCartHelpers.PrepareCart(null, new[] { taxProvider }, false, new[] { taxHelper });
            
            cart.Country = "United States";
            cart.ZipCode = "52627";

            CheckTaxes(cart.Taxes().Amount, 6.95M);
        }

        [Test]
        public void TabRatesAreParsedCorrectly() {
            var tabZipTax = new ZipCodeTaxPart();
            ContentHelpers.PreparePart(tabZipTax, "Tax");
            tabZipTax.Rates = TabRates;

            var Rates = tabZipTax.GetRates();
            var rKeys = Rates.Keys.ToArray();
            var rValues = Rates.Values.ToArray();
            Assert.That(Rates.Count, Is.EqualTo(4));
            Assert.That(rKeys[0], Is.EqualTo("52411"));
            Assert.That(rValues[0], Is.EqualTo(.07M));
            Assert.That(rKeys[1], Is.EqualTo("52627"));
            Assert.That(rValues[1], Is.EqualTo(.05M));
            Assert.That(rKeys[2], Is.EqualTo("52405"));
            Assert.That(rValues[2], Is.EqualTo(.1M));
            Assert.That(rKeys[3], Is.EqualTo("52412"));
            Assert.That(rValues[3], Is.EqualTo(.08M));
        }

        [Test]
        public void RightTaxAppliesToTabRates() {
            var tabZipTax = new ZipCodeTaxPart();
            ContentHelpers.PreparePart(tabZipTax, "Tax");
            tabZipTax.Rates = TabRates;

            var taxProvider = new FakeTaxProvider(new[] { tabZipTax });
            var taxHelper = new ZipCodeTaxComputationHelper();
            var cart = ShoppingCartHelpers.PrepareCart(null, new[] { taxProvider }, false, new[] { taxHelper });

            cart.Country = "United States";
            cart.ZipCode = "52412";

            CheckTaxes(cart.Taxes().Amount, 11.12M);
        }

        [Test]
        public void TaxDoesNotApplyToNonMatchingZip() {
            var csvZipTax = new ZipCodeTaxPart();
            ContentHelpers.PreparePart(csvZipTax, "Tax");
            csvZipTax.Rates = CsvRates;

            var taxProvider = new FakeTaxProvider(new[] { csvZipTax });
            var cart = ShoppingCartHelpers.PrepareCart(null, new[] { taxProvider });

            cart.Country = "United States";
            cart.ZipCode = "90210";

            var taxes = cart.Taxes();
            Assert.AreEqual(0, taxes.Amount);
            Assert.IsNull(taxes.Name);
        }

        private static void CheckTaxes(decimal actualTax, decimal expectedTax) {
            const double epsilon = 0.001;
            Assert.That(
                    Math.Abs(expectedTax - actualTax),
                    Is.LessThan(epsilon));
        }

        private class FakeTaxProvider : ITaxProvider {
            private readonly IEnumerable<ZipCodeTaxPart> _taxes;

            public FakeTaxProvider(IEnumerable<ZipCodeTaxPart> taxes) {
                _taxes = taxes;
            }

            public string Name { get { return "FakeTaxProvider"; } }
            public string ContentTypeName { get { return ""; } }
            public IEnumerable<ITax> GetTaxes() {
                return _taxes;
            }
        }
    }
}
