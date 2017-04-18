using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nwazet.Commerce.Services {
    public class Currency {
        public static readonly Dictionary<string, Currency> Currencies = new Dictionary<string, Currency>(InitializeIsoCurrencies());

        private static readonly string CurrencySign = CultureInfo.InvariantCulture.NumberFormat.CurrencySymbol;

        private static IDictionary<string, Currency> InitializeIsoCurrencies() {
            return new Dictionary<string, Currency> {
                { "AED", new Currency("AED", "United Arab Emirates dirham", "د.إ", decimalDigits: 2) },
                { "AFN", new Currency("AFN", "Afghan afghani", "؋", decimalDigits: 2) },
                { "ALL", new Currency("ALL", "Albanian lek", "Lek", decimalDigits: 2) },
                { "AMD", new Currency("AMD", "Armenian dram", "֏", decimalDigits: 2) },
                { "ANG", new Currency("ANG", "Netherlands Antillean guilder", "ƒ", decimalDigits: 2) },
                { "AOA", new Currency("AOA", "Angolan kwanza", "Kz", decimalDigits: 2) },
                { "ARS", new Currency("ARS", "Argentine peso", "$", decimalDigits: 2) },
                { "AUD", new Currency("AUD", "Australian dollar", "$", decimalDigits: 2) },
                { "AWG", new Currency("AWG", "Aruban florin", "ƒ", decimalDigits: 2) },
                { "AZN", new Currency("AZN", "Azerbaijani manat", "ман", decimalDigits: 2) },
                { "BAM", new Currency("BAM", "Bosnia and Herzegovina convertible mark", "KM", decimalDigits: 2) },
                { "BBD", new Currency("BBD", "Barbados dollar", "$", decimalDigits: 2) },
                { "BDT", new Currency("BDT", "Bangladeshi taka", "৳", decimalDigits: 2) },
                { "BGN", new Currency("BGN", "Bulgarian lev", "лв.", decimalDigits: 2) },
                { "BHD", new Currency("BHD", "Bahraini dinar", "د.ب.", decimalDigits: 3) },
                { "BIF", new Currency("BIF", "Burundi franc", "FBu", decimalDigits: 0) },
                { "BMD", new Currency("BMD", "Bermudian dollar", "$", decimalDigits: 2) },
                { "BND", new Currency("BND", "Brunei dollar", "$", decimalDigits: 2) },
                { "BOB", new Currency("BOB", "Boliviano", "Bs.", decimalDigits: 2) },
                { "BRL", new Currency("BRL", "Brazilian real", "R$", decimalDigits: 2) },
                { "BSD", new Currency("BSD", "Bahamian dollar", "$", decimalDigits: 2) },
                { "BTN", new Currency("BTN", "Bhutanese ngultrum", "Nu.", decimalDigits: 2) },
                { "BWP", new Currency("BWP", "Botswana pula", "P", decimalDigits: 2) },
                { "BYN", new Currency("BYN", "Belarusian ruble", "Br", decimalDigits: 2) },
                { "BZD", new Currency("BZD", "Belize dollar", "BZ$", decimalDigits: 2) },
                { "CAD", new Currency("CAD", "Canadian dollar", "$", decimalDigits: 2) },
                { "CDF", new Currency("CDF", "Congolese franc", "FC", decimalDigits: 2) },
                { "CHF", new Currency("CHF", "Swiss franc", "CHF", decimalDigits: 2) },
                { "CLP", new Currency("CLP", "Chilean peso", "$", decimalDigits: 0) },
                { "CNY", new Currency("CNY", "Yuan Renminbi", "¥", decimalDigits: 2) },
                { "COP", new Currency("COP", "Colombian peso", "$", decimalDigits: 2) },
                { "CRC", new Currency("CRC", "Costa Rican colon", "₡", decimalDigits: 2) },
                { "CUC", new Currency("CUC", "Cuban convertible peso", "CUC$", decimalDigits: 2) },
                { "CUP", new Currency("CUP", "Cuban peso", "$", decimalDigits: 2) },
                { "CVE", new Currency("CVE", "Cape Verde escudo", "$", decimalDigits: 2) },
                { "CZK", new Currency("CZK", "Czech koruna", "Kč", decimalDigits: 2) },
                { "DJF", new Currency("DJF", "Djiboutian franc", "Fdj", decimalDigits: 0) },
                { "DKK", new Currency("DKK", "Danish krone", "kr.", decimalDigits: 2) },
                { "DOP", new Currency("DOP", "Dominican peso", "RD$", decimalDigits: 2) },
                { "DZD", new Currency("DZD", "Algerian dinar", "DA", decimalDigits: 2) }, 
                { "EGP", new Currency("EGP", "Egyptian pound", "E£", decimalDigits: 2) },
                { "ERN", new Currency("ERN", "Eritrean nakfa", "ERN", decimalDigits: 2) },
                { "ETB", new Currency("ETB", "Ethiopian birr", "Br", decimalDigits: 2) },
                { "EUR", new Currency("EUR", "Euro", "€", decimalDigits: 2) },
                { "FJD", new Currency("FJD", "Fiji dollar", "FJ$", decimalDigits: 2) },
                { "FKP", new Currency("FKP", "Falkland Islands pound", "£", decimalDigits: 2) },
                { "GBP", new Currency("GBP", "Pound sterling", "£", decimalDigits: 2) },
                { "GEL", new Currency("GEL", "Georgian lari", "ლ", decimalDigits: 2) },
                { "GHS", new Currency("GHS", "Ghanaian cedi", "GH¢", decimalDigits: 2) },
                { "GIP", new Currency("GIP", "Gibraltar pound", "£", decimalDigits: 2) },
                { "GMD", new Currency("GMD", "Gambian dalasi", "D", decimalDigits: 2) },
                { "GNF", new Currency("GNF", "Guinea franc", "FG", decimalDigits: 0) },
                { "GTQ", new Currency("GTQ", "Guatemalan quetzal", "Q", decimalDigits: 2) },
                { "GYD", new Currency("GYD", "Guyanese dollar", "G$", decimalDigits: 2) },
                { "HKD", new Currency("HKD", "Hong Kong dollar", "HK$", decimalDigits: 2) },
                { "HNL", new Currency("HNL", "Honduran lempira", "L", decimalDigits: 2) },
                { "HRK", new Currency("HRK", "Croatian kuna", "kn", decimalDigits: 2) },
                { "HTG", new Currency("HTG", "Haitian gourde", "G", decimalDigits: 2) },
                { "HUF", new Currency("HUF", "Hungarian forint", "Ft", decimalDigits: 2) },
                { "IDR", new Currency("IDR", "Indonesian rupiah", "Rp", decimalDigits: 2) },
                { "ILS", new Currency("ILS", "Israeli new shekel", "₪", decimalDigits: 2) },
                { "INR", new Currency("INR", "Indian rupee", "₹", decimalDigits: 2) },
                { "IQD", new Currency("IQD", "Iraqi dinar", "د.ع", decimalDigits: 3) },
                { "IRR", new Currency("IRR", "Iranian rial", "ريال", decimalDigits: 2) },
                { "ISK", new Currency("ISK", "Icelandic króna", "kr", decimalDigits: 0) },
                { "JMD", new Currency("JMD", "Jamaican dollar", "J$", decimalDigits: 2) },
                { "JOD", new Currency("JOD", "Jordanian dinar", "د.ا.‏", decimalDigits: 3) },
                { "JPY", new Currency("JPY", "Japanese yen", "¥", decimalDigits: 0) },
                { "KES", new Currency("KES", "Kenyan shilling", "KSh", decimalDigits: 2) },
                { "KGS", new Currency("KGS", "Kyrgyzstani som", "сом", decimalDigits: 2) },
                { "KHR", new Currency("KHR", "Cambodian riel", "៛", decimalDigits: 2) },
                { "KMF", new Currency("KMF", "Comoro franc", "CF", decimalDigits: 0) },
                { "KPW", new Currency("KPW", "North Korean won", "₩", decimalDigits: 2) },
                { "KRW", new Currency("KRW", "South Korean won", "₩", decimalDigits: 0) },
                { "KWD", new Currency("KWD", "Kuwaiti dinar", "د.ك", decimalDigits: 3) },
                { "KYD", new Currency("KYD", "Cayman Islands dollar", "$", decimalDigits: 2) },
                { "KZT", new Currency("KZT", "Kazakhstani tenge", "₸", decimalDigits: 2) },
                { "LAK", new Currency("LAK", "Lao kip", "₭", decimalDigits: 2) },
                { "LBP", new Currency("LBP", "Lebanese pound", "ل.ل", decimalDigits: 2) },
                { "LKR", new Currency("LKR", "Sri Lankan rupee", "Rs", decimalDigits: 2) },
                { "LRD", new Currency("LRD", "Liberian dollar", "L$", decimalDigits: 2) },
                { "LSL", new Currency("LSL", "Lesotho loti", "L", decimalDigits: 2) }, 
                { "LYD", new Currency("LYD", "Libyan dinar", "ل.د", decimalDigits: 3) },
                { "MAD", new Currency("MAD", "Moroccan dirham", "د.م.", decimalDigits: 2) },
                { "MDL", new Currency("MDL", "Moldovan leu", "L", decimalDigits: 2) },
                { "MGA", new Currency("MGA", "Malagasy ariary", "Ar", decimalDigits: 2) },
                { "MKD", new Currency("MKD", "Macedonian denar", "ден", decimalDigits: 2) },
                { "MMK", new Currency("MMK", "Myanma kyat", "K", decimalDigits: 2) },
                { "MNT", new Currency("MNT", "Mongolian tugrik", "₮", decimalDigits: 2) },
                { "MOP", new Currency("MOP", "Macanese pataca", "MOP$", decimalDigits: 2) },
                { "MRO", new Currency("MRO", "Mauritanian ouguiya", "UM", decimalDigits: 2) },
                { "MUR", new Currency("MUR", "Mauritian rupee", "Rs", decimalDigits: 2) },
                { "MVR", new Currency("MVR", "Maldivian rufiyaa", "Rf", decimalDigits: 2) },
                { "MWK", new Currency("MWK", "Malawi kwacha", "MK", decimalDigits: 2) },
                { "MXN", new Currency("MXN", "Mexican peso", "$", decimalDigits: 2) },
                { "MYR", new Currency("MYR", "Malaysian ringgit", "RM", decimalDigits: 2) },
                { "MZN", new Currency("MZN", "Mozambican metical", "MTn", decimalDigits: 2) },
                { "NAD", new Currency("NAD", "Namibian dollar", "N$", decimalDigits: 2) }, 
                { "NGN", new Currency("NGN", "Nigerian naira", "₦", decimalDigits: 2) },
                { "NIO", new Currency("NIO", "Nicaraguan córdoba oro", "C$", decimalDigits: 2) },
                { "NOK", new Currency("NOK", "Norwegian krone", "kr", decimalDigits: 2) },
                { "NPR", new Currency("NPR", "Nepalese rupee", "Rs", decimalDigits: 2) },
                { "NZD", new Currency("NZD", "New Zealand dollar", "$", decimalDigits: 2) },
                { "OMR", new Currency("OMR", "Omani rial", "ر.ع.", decimalDigits: 3) },
                { "PAB", new Currency("PAB", "Panamanian balboa", "B/.", decimalDigits: 2) },
                { "PEN", new Currency("PEN", "Peruvian sol", "S/.", decimalDigits: 2) },
                { "PGK", new Currency("PGK", "Papua New Guinean kina", "K", decimalDigits: 2) },
                { "PHP", new Currency("PHP", "Philippine peso", "₱", decimalDigits: 2) },
                { "PKR", new Currency("PKR", "Pakistani rupee", "Rs", decimalDigits: 2) },
                { "PLN", new Currency("PLN", "Polish złoty", "zł", decimalDigits: 2) },
                { "PYG", new Currency("PYG", "Paraguayan guaraní", "₲", decimalDigits: 0) },
                { "QAR", new Currency("QAR", "Qatari riyal", "ر.ق", decimalDigits: 2) }, 
                { "RON", new Currency("RON", "Romanian new leu", "lei", decimalDigits: 2) },
                { "RSD", new Currency("RSD", "Serbian dinar", "РСД", decimalDigits: 2) },
                { "RUB", new Currency("RUB", "Russian rouble", "₽", decimalDigits: 2) },
                { "RWF", new Currency("RWF", "Rwandan franc", "RFw", decimalDigits: 0) },
                { "SAR", new Currency("SAR", "Saudi riyal", "ر.س", decimalDigits: 2) },
                { "SBD", new Currency("SBD", "Solomon Islands dollar", "SI$", decimalDigits: 2) },
                { "SCR", new Currency("SCR", "Seychelles rupee", "SR", decimalDigits: 2) },
                { "SDG", new Currency("SDG", "Sudanese pound", "ج.س.", decimalDigits: 2) },
                { "SEK", new Currency("SEK", "Swedish krona/kronor", "kr", decimalDigits: 2) },
                { "SGD", new Currency("SGD", "Singapore dollar", "S$", decimalDigits: 2) },
                { "SHP", new Currency("SHP", "Saint Helena pound", "£", decimalDigits: 2) },
                { "SLL", new Currency("SLL", "Sierra Leonean leone", "Le", decimalDigits: 2) },
                { "SOS", new Currency("SOS", "Somali shilling", "S", decimalDigits: 2) },
                { "SRD", new Currency("SRD", "Surinamese dollar", "$", decimalDigits: 2) },
                { "SSP", new Currency("SSP", "South Sudanese pound", "£", decimalDigits: 2) }, 
                { "STD", new Currency("STD", "São Tomé and Príncipe dobra", "Db", decimalDigits: 2) },
                { "SVC", new Currency("SVC", "El Salvador Colon", "₡", decimalDigits: 2) },
                { "SYP", new Currency("SYP", "Syrian pound", "ܠ.ܣ.‏", decimalDigits: 2) },
                { "SZL", new Currency("SZL", "Swazi lilangeni", "L", decimalDigits: 2) },
                { "THB", new Currency("THB", "Thai baht", "฿", decimalDigits: 2) },
                { "TJS", new Currency("TJS", "Tajikistani somoni", "смн", decimalDigits: 2) },
                { "TMT", new Currency("TMT", "Turkmenistani new manat", "m", decimalDigits: 2) }, 
                { "TND", new Currency("TND", "Tunisian dinar", "د.ت", decimalDigits: 3) }, 
                { "TOP", new Currency("TOP", "Tongan paʻanga", "T$", decimalDigits: 2) },
                { "TRY", new Currency("TRY", "Turkish lira", "₺", decimalDigits: 2) },
                { "TTD", new Currency("TTD", "Trinidad and Tobago dollar", "$", decimalDigits: 2) },
                { "TWD", new Currency("TWD", "New Taiwan dollar", "NT$", decimalDigits: 2) },
                { "TZS", new Currency("TZS", "Tanzanian shilling", "x/y", decimalDigits: 2) }, 
                { "UAH", new Currency("UAH", "Ukrainian hryvnia", "₴", decimalDigits: 2) },
                { "UGX", new Currency("UGX", "Ugandan shilling", "USh", decimalDigits: 0) },
                { "USD", new Currency("USD", "United States dollar", "$", decimalDigits: 2) },
                { "UYU", new Currency("UYU", "Uruguayan peso", "$", decimalDigits: 2) },
                { "UZS", new Currency("UZS", "Uzbekistan sum", "лв", decimalDigits: 2) },
                { "VEF", new Currency("VEF", "Venezuelan bolívar", "Bs.", decimalDigits: 2) },
                { "VND", new Currency("VND", "Vietnamese dong", "₫", decimalDigits: 0) },
                { "VUV", new Currency("VUV", "Vanuatu vatu", "VT", decimalDigits: 0) },
                { "WST", new Currency("WST", "Samoan tala", "WS$", decimalDigits: 2) },
                { "XAF", new Currency("XAF", "CFA franc BEAC", "FCFA", decimalDigits: 0) },
                { "XCD", new Currency("XCD", "East caribbean dollar", "", decimalDigits: 2)},
                { "XOF", new Currency("XOF", "CFA franc BCEAO", "FCFA", decimalDigits: 0) },
                { "XPF", new Currency("XPF", "CFP franc", "", decimalDigits: 0) },
                { "YER", new Currency("YER", "Yemeni rial", "﷼", decimalDigits: 2) },
                { "ZAR", new Currency("ZAR", "South African rand", "R", decimalDigits: 2) },
                { "ZMW", new Currency("ZMW", "Zambian kwacha", "ZK", decimalDigits: 2) },
                { "ZWL", new Currency("ZWL", "Zimbabwean dollar", "$", decimalDigits: 2) }
            };
        }

        public string CurrencyCode { get; set; } //3-letter ISO 4217 code
        public string CurrencyName { get; set; } //ISO 4217 currency name
        public string Symbol { get; set; } //Symbol used for the currency
        public int DecimalDigits { get; set; }

        /// <summary>
        /// Creates a new instance of Currency
        /// </summary>
        /// <param name="currencyCode">The ISO-4217 code identifying the currency</param>
        /// <param name="currencyName">The name of the currency</param>
        /// <param name="symbol">The symbol to be used for the currency</param>
        /// <param name="decimalDigits">The number of decimal places to use</param>
        public Currency(string currencyCode, string currencyName, string symbol,
            int decimalDigits) {

            CurrencyCode = currencyCode;
            CurrencyName = currencyName;
            Symbol = symbol;
            DecimalDigits = decimalDigits;
        }

        /// <summary>
        /// Returns a CultureInfo object based on the one provided and with the formatting information taken from this
        /// Currency object.
        /// </summary>
        /// <param name="baseCulture">The culture to start from.</param>
        /// <returns>The culture with the correct formatting for this currency.</returns>
        public CultureInfo GetCulture(CultureInfo baseCulture) {
            CultureInfo clone = (CultureInfo)baseCulture.Clone();
            clone.NumberFormat.CurrencyDecimalDigits = DecimalDigits;
            clone.NumberFormat.CurrencySymbol = Symbol;
            return clone;
        }
        /// <summary>
        /// Returns a CultureInfo object based on the culteure identified by the string provided, and with the formatting
        /// information taken from this Currency object.
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public CultureInfo GetCulture(string culture) {
            //GetCultureInfo throws a CultureNotFoundException if the string is not valid
            CultureInfo cul = CultureInfo.GetCultureInfo(culture);
            return GetCulture(cul);
        }

        /// <summary>
        /// Formats an amount of money with culture information adapted from the current currency.
        /// </summary>
        /// <param name="price">The amount of money.</param>
        /// <param name="culture">The CultureInfo object describing the base culture to use.</param>
        /// <returns>A string representation of the money amount.</returns>
        public string PriceAsString(double price, CultureInfo culture) {
            return price.ToString("c", GetCulture(culture));
        }
        /// <summary>
        /// Formats an amount of money with culture information adapted from the current currency.
        /// </summary>
        /// <param name="price">The amount of money.</param>
        /// <param name="culture">The string describing the base culture to use.</param>
        /// <returns>A string representation of the money amount.</returns>
        public string PriceAsString(double price, string culture) {
            return price.ToString("c", GetCulture(culture));
        }
        /// <summary>
        /// Formats an amount of money with culture information adapted from the current currency.
        /// </summary>
        /// <param name="price">The amount of money.</param>
        /// <param name="culture">The CultureInfo object describing the base culture to use.</param>
        /// <returns>A string representation of the money amount.</returns>
        public string PriceAsString(decimal price, CultureInfo culture) {
            return price.ToString("c", GetCulture(culture));
        }
        /// <summary>
        /// Formats an amount of money with culture information adapted from the current currency.
        /// </summary>
        /// <param name="price">The amount of money.</param>
        /// <param name="culture">The string describing the base culture to use.</param>
        /// <returns>A string representation of the money amount.</returns>
        public string PriceAsString(decimal price, string culture) {
            return price.ToString("c", GetCulture(culture));
        }
    }
}
