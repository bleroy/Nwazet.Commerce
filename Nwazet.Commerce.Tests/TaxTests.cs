using System.Collections.Generic;
using NUnit.Framework;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.Tests.Helpers;

namespace Nwazet.Commerce.Tests
{
    class TaxTests
    {
        [Test]
        public void RightTaxAppliesToRightState() {
            var oregonTax = GetOregonTax();
            var washingtonTax = GetWashingtonTax();
            var taxProvider = new FakeTaxProvider(new[] {oregonTax, washingtonTax});
            var cart = ShoppingCartHelpers.PrepareCart(null, new[] {taxProvider});
            cart.Country = Country.UnitedStates;
            var subtotal = cart.Subtotal();

            cart.ZipCode = "98008";
            Assert.That(cart.Taxes().Amount, Is.EqualTo(subtotal * washingtonTax.Rate));

            cart.ZipCode = "97218";
            Assert.That(cart.Taxes().Amount, Is.EqualTo(subtotal * oregonTax.Rate));

            cart.ZipCode = "92210";

            var taxes = cart.Taxes();
            Assert.That(taxes.Name, Is.Null);
            Assert.That(taxes.Amount, Is.EqualTo(0));
        }

        [Test]
        public void AnyStateTaxAppliesToAnyStateButNotOtherCountries()
        {
            var anyStateTax = GetAnyStateTax();
            var frenchTax = GetFrenchTax();
            var taxProvider = new FakeTaxProvider(new[] { anyStateTax, frenchTax });
            var cart = ShoppingCartHelpers.PrepareCart(null, new[] { taxProvider });
            cart.Country = Country.UnitedStates;
            var subtotal = cart.Subtotal();

            cart.ZipCode = "98008";
            Assert.That(cart.Taxes().Amount, Is.EqualTo(subtotal * anyStateTax.Rate));

            cart.ZipCode = "97218";
            Assert.That(cart.Taxes().Amount, Is.EqualTo(subtotal * anyStateTax.Rate));

            cart.Country = "France";
            Assert.That(cart.Taxes().Amount, Is.EqualTo(subtotal * frenchTax.Rate));
        }

        [Test]
        public void AnyCountryTaxAppliesToAnyCountry()
        {
            var anyCountryTax = GetAnyCountryTax();
            var taxProvider = new FakeTaxProvider(new[] { anyCountryTax });
            var cart = ShoppingCartHelpers.PrepareCart(null, new[] { taxProvider });
            var subtotal = cart.Subtotal();

            cart.Country = Country.UnitedStates;
            cart.ZipCode = "98008";
            Assert.That(cart.Taxes().Amount, Is.EqualTo(subtotal * anyCountryTax.Rate));

            cart.Country = "France";
            Assert.That(cart.Taxes().Amount, Is.EqualTo(subtotal * anyCountryTax.Rate));
        }

        [Test]
        public void RightTaxAppliesToRightCountry()
        {
            var frenchTax = GetFrenchTax();
            var britishTax = GetBritishTax();
            var washingtonTax = GetWashingtonTax();
            var oregonTax = GetOregonTax();
            var taxProvider = new FakeTaxProvider(new[] { washingtonTax, britishTax, frenchTax, oregonTax });
            var cart = ShoppingCartHelpers.PrepareCart(null, new[] { taxProvider });
            var subtotal = cart.Subtotal();

            cart.Country = frenchTax.Country;
            Assert.That(cart.Taxes().Amount, Is.EqualTo(subtotal * frenchTax.Rate));

            cart.Country = britishTax.Country;
            Assert.That(cart.Taxes().Amount, Is.EqualTo(subtotal * britishTax.Rate));

            cart.Country = Country.UnitedStates;

            var taxes = cart.Taxes();
            Assert.That(taxes.Name, Is.Null);
            Assert.That(taxes.Amount, Is.EqualTo(0));
        }

        [Test]
        public void TaxDoesntApplyToDifferentCountry()
        {
            var oregonTax = GetOregonTax();
            var taxProvider = new FakeTaxProvider(new[] { oregonTax });
            var cart = ShoppingCartHelpers.PrepareCart(null, new[] { taxProvider });
            cart.Country = "France";
            cart.ZipCode = "97218";

            var taxes = cart.Taxes();
            Assert.That(taxes.Name, Is.Null);
            Assert.That(taxes.Amount, Is.EqualTo(0));
        }

        [Test]
        public void HigherPriorityWins() {
            var frenchTax1 = GetFrenchTax();
            frenchTax1.Priority = 1;
            frenchTax1.Rate = 0.1;
            var anyCountryTax = GetAnyCountryTax();
            anyCountryTax.Priority = 2;
            anyCountryTax.Rate = 0.2;
            var frenchTax2 = GetFrenchTax();
            frenchTax2.Priority = 3;
            frenchTax2.Rate = 0.3;
            var taxProvider = new FakeTaxProvider(new[] { frenchTax1, frenchTax2, anyCountryTax });
            var cart = ShoppingCartHelpers.PrepareCart(null, new[] { taxProvider });
            cart.Country = frenchTax1.Country;
            var subtotal = cart.Subtotal();

            Assert.That(cart.Taxes().Amount, Is.EqualTo(subtotal * frenchTax2.Rate));

            anyCountryTax.Priority = 4;
            Assert.That(cart.Taxes().Amount, Is.EqualTo(subtotal * anyCountryTax.Rate));
        }

        [Test]
        public void NoApplicableTaxesYieldsNoTax() {
            var frenchTax = GetFrenchTax();
            var britishTax = GetBritishTax();
            var washingtonTax = GetWashingtonTax();
            var oregonTax = GetOregonTax();
            var taxProvider = new FakeTaxProvider(new[] { washingtonTax, britishTax, frenchTax, oregonTax });
            var cart = ShoppingCartHelpers.PrepareCart(null, new[] { taxProvider });

            cart.Country = "Kazakhstan";

            var taxes = cart.Taxes();
            Assert.That(taxes.Name, Is.Null);
            Assert.That(taxes.Amount, Is.EqualTo(0));
        }

        [Test]
        public void NoNegativeTax()
        {
            var frenchTax = GetFrenchTax();
            frenchTax.Rate = -0.5;
            var taxProvider = new FakeTaxProvider(new[] { frenchTax });
            var cart = ShoppingCartHelpers.PrepareCart(null, new[] { taxProvider });

            cart.Country = frenchTax.Country;

            var taxes = cart.Taxes();
            Assert.That(taxes.Name, Is.Null);
            Assert.That(taxes.Amount, Is.EqualTo(0));
        }

        private static StateOrCountryTaxPart GetAnyStateTax()
        {
            var anyStateTax = new StateOrCountryTaxPart();
            ContentHelpers.PreparePart<StateOrCountryTaxPart, StateOrCountryTaxPartRecord>(anyStateTax, "Tax");
            anyStateTax.Country = Country.UnitedStates;
            anyStateTax.State = "*";
            anyStateTax.Rate = 0.05;
            return anyStateTax;
        }

        private static StateOrCountryTaxPart GetAnyCountryTax()
        {
            var anyCountryTax = new StateOrCountryTaxPart();
            ContentHelpers.PreparePart<StateOrCountryTaxPart, StateOrCountryTaxPartRecord>(anyCountryTax, "Tax");
            anyCountryTax.Country = "*";
            anyCountryTax.Rate = 0.07;
            return anyCountryTax;
        }

        private static StateOrCountryTaxPart GetOregonTax()
        {
            var oregonTax = new StateOrCountryTaxPart();
            ContentHelpers.PreparePart<StateOrCountryTaxPart, StateOrCountryTaxPartRecord>(oregonTax, "Tax");
            oregonTax.Country = Country.UnitedStates;
            oregonTax.State = "OR";
            oregonTax.Rate = 0.15;
            return oregonTax;
        }

        private static StateOrCountryTaxPart GetWashingtonTax() {
            var washingtonTax = new StateOrCountryTaxPart();
            ContentHelpers.PreparePart<StateOrCountryTaxPart, StateOrCountryTaxPartRecord>(washingtonTax, "Tax");
            washingtonTax.Country = Country.UnitedStates;
            washingtonTax.State = "WA";
            washingtonTax.Rate = 0.095;
            return washingtonTax;
        }

        private static StateOrCountryTaxPart GetFrenchTax() {
            var frenchTax = new StateOrCountryTaxPart();
            ContentHelpers.PreparePart<StateOrCountryTaxPart, StateOrCountryTaxPartRecord>(frenchTax, "Tax");
            frenchTax.Country = "France";
            frenchTax.Rate = 0.19;
            return frenchTax;
        }

        private static StateOrCountryTaxPart GetBritishTax()
        {
            var britishTax = new StateOrCountryTaxPart();
            ContentHelpers.PreparePart<StateOrCountryTaxPart, StateOrCountryTaxPartRecord>(britishTax, "Tax");
            britishTax.Country = "United Kingdom (Great Britain and Northern Ireland)";
            britishTax.Rate = 0.15;
            return britishTax;
        }

        private class FakeTaxProvider : ITaxProvider {
            private readonly IEnumerable<StateOrCountryTaxPart> _taxes;
 
            public FakeTaxProvider(IEnumerable<StateOrCountryTaxPart> taxes) {
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
