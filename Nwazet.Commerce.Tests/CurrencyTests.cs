using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Nwazet.Commerce.Services;

namespace Nwazet.Commerce.Tests {
    [TestFixture]
    public class CurrencyTests {
        [Test]
        public void FormatItalianCulture() {
            Currency dollar = Currency.Currencies["USD"];
            Currency euro = Currency.Currencies["EUR"];
            Currency chileanPeso = Currency.Currencies["CLP"]; //currency with 0 decimal digits
            Currency bahrainiDinar = Currency.Currencies["BHD"]; //currency with 3 decimal digits

            double amount = 123.456;

            CultureInfo italian = CultureInfo.GetCultureInfo("it-IT");

            Assert.That(dollar.CurrencyName, Is.EqualTo("United States dollar"));
            Assert.That(euro.CurrencyName, Is.EqualTo("Euro"));
            Assert.That(chileanPeso.CurrencyName, Is.EqualTo("Chilean peso"));
            Assert.That(bahrainiDinar.CurrencyName, Is.EqualTo("Bahraini dinar"));

            Assert.That(dollar.PriceAsString(amount, italian), Is.EqualTo("123,46 $"));
            Assert.That(euro.PriceAsString(amount, italian), Is.EqualTo("123,46 €"));
            Assert.That(chileanPeso.PriceAsString(amount, italian), Is.EqualTo("123 $"));
            Assert.That(bahrainiDinar.PriceAsString(amount, italian), Is.EqualTo("123,456 د.ب."));
        }

        [Test]
        public void FormatUSCulture() {
            Currency dollar = Currency.Currencies["USD"];
            Currency euro = Currency.Currencies["EUR"];
            Currency chileanPeso = Currency.Currencies["CLP"]; //currency with 0 decimal digits
            Currency bahrainiDinar = Currency.Currencies["BHD"]; //currency with 3 decimal digits

            double amount = 123.456;

            CultureInfo american = CultureInfo.GetCultureInfo("en-US");

            Assert.That(dollar.CurrencyName, Is.EqualTo("United States dollar"));
            Assert.That(euro.CurrencyName, Is.EqualTo("Euro"));
            Assert.That(chileanPeso.CurrencyName, Is.EqualTo("Chilean peso"));
            Assert.That(bahrainiDinar.CurrencyName, Is.EqualTo("Bahraini dinar"));

            Assert.That(dollar.PriceAsString(amount, american), Is.EqualTo("$123.46"));
            Assert.That(euro.PriceAsString(amount, american), Is.EqualTo("€123.46"));
            Assert.That(chileanPeso.PriceAsString(amount, american), Is.EqualTo("$123"));
            Assert.That(bahrainiDinar.PriceAsString(amount, american), Is.EqualTo("د.ب.123.456")); //the currency symbol is before the amount
        }
    }
}
