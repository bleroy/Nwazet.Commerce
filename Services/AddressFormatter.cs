using System;
using System.Collections.Generic;
using System.Linq;
using Nwazet.Commerce.Models;

namespace Nwazet.Commerce.Services {
    public class AddressFormatter : IAddressFormatter {
        public string Format(Address address) {
            string country = address.Country ?? Country.UnitedStates;
            string pattern;
            if (!_addressPatterns.TryGetValue(country, out pattern)) {
                pattern = DefaultPattern;
            }
            var rawString = pattern
                .Replace("{Honorific}", address.Honorific)
                .Replace("{FirstName}", address.FirstName)
                .Replace("{LastName}", address.LastName)
                .Replace("{Address1}", address.Address1)
                .Replace("{Address2}", address.Address2)
                .Replace("{City}", address.City)
                .Replace("{Company}", address.Company)
                .Replace("{Country}", address.Country)
                .Replace("{PostalCode}", address.PostalCode)
                .Replace("{Province}", address.Province);
            return string.Join(
                Environment.NewLine,
                rawString
                    .Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => s != ""));
        }

        private const string DefaultPattern = @"{Honorific} {FirstName} {LastName}
{Company}
{Address1}
{Address2}
{City}, {Province} {PostalCode}
{Country}";
        private const string ChinesePattern = @"{Honorific} {LastName} {FirstName}
{Company}
{Address1}
{Address2}
{City}
{PostalCode} {Province} 
{Country}";
        private const string BrazilianPattern = @"{Company}
{Honorific} {FirstName} {LastName}
{Address1}
{Address2}
{City}-{Province} 
{PostalCode}
{Country}";
        private const string BulgarianPattern = @"{Company}
{Honorific} {FirstName}  {LastName}
{Address1}
{Address2}
{PostalCode} {City}
{Province}
{Country}";
        private const string EuropeanPattern = @"{Honorific} {FirstName} {LastName}
{Company}
{Address1}
{Address2}
{PostalCode} {City}
{Province} 
{Country}";
        private const string GermanPattern = @"{Company}
{Honorific} {FirstName}  {LastName}
{Address1}
{Address2}
{PostalCode} {City}
{Country}";
        private const string HungarianPattern = @"{Honorific} {LastName} {FirstName}
{Company}
{City}
{Address1}
{Address2}
{PostalCode}
{Province}
{Country}";
        private const string ItalianPattern = @"{Honorific} {FirstName} {LastName}
{Company}
{Address1}
{Address2}
{PostalCode} {City} {Province}
{Country}";
        private const string JapanesePattern = @"{LastName} {FirstName} {Honorific}
{Company}
{Address1}
{Address2}
{City}
{Province} {PostalCode}
{Country}";
        private const string KoreanPattern = @"{Honorific} {FirstName} {LastName}
{Company}
{Address1}
{Address2}
{City} {PostalCode}
{Province}
{Country}";
        private const string MalaysianPattern = @"{Honorific} {FirstName} {LastName}
{Company}
{Address1}
{Address2}
{PostalCode} {City}
{Province}
{Country}";
        private const string PortuguesePattern = @"{Honorific} {FirstName} {LastName}
{Company}
{Address1}
{Address2}
{City}
{PostalCode}
{Country}";
        private const string RussianPattern = @"{LastName} {FirstName}
{Company}
{Address1}
{Address2}
{City}
{Province} 
{PostalCode}
{Country}";

        private readonly Dictionary<string, string> _addressPatterns =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                {"", DefaultPattern},
                {"brazil", BrazilianPattern},
                {"bulgaria", BulgarianPattern},
                {"china", ChinesePattern},
                {"croatia", EuropeanPattern},
                {"czech republic", EuropeanPattern},
                {"denmark", EuropeanPattern},
                {"finland", EuropeanPattern},
                {"france", EuropeanPattern},
                {"germany", GermanPattern},
                {"greece", EuropeanPattern},
                {"hungary", HungarianPattern},
                {"italy", ItalianPattern},
                {"japan", JapanesePattern},
                {"korea", KoreanPattern},
                {"malaysia", MalaysianPattern},
                {"netherlands", EuropeanPattern},
                {"norway", EuropeanPattern},
                {"poland", EuropeanPattern},
                {"portugal", PortuguesePattern},
                {"romania", EuropeanPattern},
                {"russia", RussianPattern},
                {"serbia", EuropeanPattern},
                {"slovenia", EuropeanPattern},
                {"spain", EuropeanPattern},
                {"sweden", EuropeanPattern},
                {"switzerland", EuropeanPattern},
                {"turkey", EuropeanPattern},
            };

        public string FullName(Address address) {
            string country = address.Country ?? Country.UnitedStates;
            string pattern;
            if (!_namePatterns.TryGetValue(country, out pattern)) {
                pattern = DefaultNamePattern;
            }
            return pattern
                .Replace("{Honorific}", address.Honorific)
                .Replace("{FirstName}", address.FirstName)
                .Replace("{LastName}", address.LastName);
        }

        private const string DefaultNamePattern = @"{Honorific} {FirstName} {LastName}";
        private const string EastAsianNamePattern = @"{LastName} {FirstName} {Honorific}";
        private const string HungarianNamePattern = @"{Honorific} {LastName} {FirstName}";
        private const string RussianNamePattern = @"{LastName} {FirstName}";

        private readonly Dictionary<string, string> _namePatterns =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                {"", DefaultNamePattern},
                {"china", EastAsianNamePattern},
                {"japan", EastAsianNamePattern},
                {"korea", EastAsianNamePattern},
                {"hungary", HungarianNamePattern},
                {"russia", RussianNamePattern}
            };
    }
}