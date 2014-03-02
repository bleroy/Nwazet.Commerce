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
        private static readonly string CsvRates = "52411,.07\r\n52627,.05\r\n52405,.1\r\n52412,.08";
        private static readonly string TabRates = "52411\t.07\n52627\t.05\n52405\t.1\n52412\t.08";

        [Test]
        public void RightTaxAppliesToCsvRates() {
            var csvZipTax = new ZipCodeTaxPart();
            ContentHelpers.PreparePart<ZipCodeTaxPart>(csvZipTax, "Tax");
            csvZipTax.Rates = CsvRates;
            
            var taxProvider = new FakeTaxProvider(new[] { csvZipTax });
            var cart = ShoppingCartHelpers.PrepareCart(null, new[] { taxProvider });
            
            var subtotal = cart.Subtotal();
            cart.Country = "United States";
            cart.ZipCode = "52627";

            CheckTaxes(cart.Taxes().Amount, 0.05);
        }

        [Test]
        public void RightTaxAppliesToTabRates() {
            var tabZipTax = new ZipCodeTaxPart();
            ContentHelpers.PreparePart<ZipCodeTaxPart>(tabZipTax, "Tax");
            tabZipTax.Rates = TabRates;

            var taxProvider = new FakeTaxProvider(new[] { tabZipTax });
            var cart = ShoppingCartHelpers.PrepareCart(null, new[] { taxProvider });

            var subtotal = cart.Subtotal();
            cart.Country = "United States";
            cart.ZipCode = "52412";

            CheckTaxes(cart.Taxes().Amount, 0.08);
        }

        [Test]
        public void TaxDoesNotApplyToNonMatchingZip() {
            var csvZipTax = new ZipCodeTaxPart();
            ContentHelpers.PreparePart<ZipCodeTaxPart>(csvZipTax, "Tax");
            csvZipTax.Rates = CsvRates;

            var taxProvider = new FakeTaxProvider(new[] { csvZipTax });
            var cart = ShoppingCartHelpers.PrepareCart(null, new[] { taxProvider });

            var subtotal = cart.Subtotal();
            cart.Country = "United States";
            cart.ZipCode = "90210";

            Assert.IsNull(cart.Taxes());
        }

        private static void CheckTaxes(double actualTax, double expectedTax) {
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
