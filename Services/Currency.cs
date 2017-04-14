using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nwazet.Commerce.Services {
    public class Currency {
        /// <summary>
        /// This is a list of iso4217 codes, updated on 2017/03/22 (March 22, 2017).
        /// For the list: http://www.xe.com/iso4217.php
        /// For currency symbols: http://www.xe.com/symbols.php
        /// Each element in this array is a single string representing a pair of currency code and description.
        /// The formatting is "CCC description":
        ///  - The first 3 characters are the currency code
        ///  - The fourth character is a white space
        ///  - Everything after that is the desscription
        /// </summary>
        #region Static list of codes
        private static readonly string[] ISO4217 = {
            "AED United Arab Emirates Dirham",
            "AFN Afghanistan Afghani",
            "ALL Albania Lek",
            "AMD Armenia Dram",
            "ANG Netherlands Antilles Guilder",
            "AOA Angola Kwanza",
            "ARS Argentina Peso",
            "AUD Australia Dollar",
            "AWG Aruba Guilder",
            "AZN Azerbaijan New Manat",
            "BAM Bosnia and Herzegovina Convertible Marka",
            "BBD Barbados Dollar",
            "BDT Bangladesh Taka",
            "BGN Bulgaria Lev",
            "BHD Bahrain Dinar",
            "BIF Burundi Franc",
            "BMD Bermuda Dollar",
            "BND Brunei Darussalam Dollar",
            "BOB Bolivia Bolíviano",
            "BRL Brazil Real",
            "BSD Bahamas Dollar",
            "BTN Bhutan Ngultrum",
            "BWP Botswana Pula",
            "BYN Belarus Ruble",
            "BZD Belize Dollar",
            "CAD Canada Dollar",
            "CDF Congo/Kinshasa Franc",
            "CHF Switzerland Franc",
            "CLP Chile Peso",
            "CNY China Yuan Renminbi",
            "COP Colombia Peso",
            "CRC Costa Rica Colon",
            "CUC Cuba Convertible Peso",
            "CUP Cuba Peso",
            "CVE Cape Verde Escudo",
            "CZK Czech Republic Koruna",
            "DJF Djibouti Franc",
            "DKK Denmark Krone",
            "DOP Dominican Republic Peso",
            "DZD Algeria Dinar",
            "EGP Egypt Pound",
            "ERN Eritrea Nakfa",
            "ETB Ethiopia Birr",
            "EUR Euro Member Countries",
            "FJD Fiji Dollar",
            "FKP Falkland Islands (Malvinas) Pound",
            "GBP United Kingdom Pound",
            "GEL Georgia Lari",
            "GGP Guernsey Pound",
            "GHS Ghana Cedi",
            "GIP Gibraltar Pound",
            "GMD Gambia Dalasi",
            "GNF Guinea Franc",
            "GTQ Guatemala Quetzal",
            "GYD Guyana Dollar",
            "HKD Hong Kong Dollar",
            "HNL Honduras Lempira",
            "HRK Croatia Kuna",
            "HTG Haiti Gourde",
            "HUF Hungary Forint",
            "IDR Indonesia Rupiah",
            "ILS Israel Shekel",
            "IMP Isle of Man Pound",
            "INR India Rupee",
            "IQD Iraq Dinar",
            "IRR Iran Rial",
            "ISK Iceland Krona",
            "JEP Jersey Pound",
            "JMD Jamaica Dollar",
            "JOD Jordan Dinar",
            "JPY Japan Yen",
            "KES Kenya Shilling",
            "KGS Kyrgyzstan Som",
            "KHR Cambodia Riel",
            "KMF Comoros Franc",
            "KPW Korea (North) Won",
            "KRW Korea (South) Won",
            "KWD Kuwait Dinar",
            "KYD Cayman Islands Dollar",
            "KZT Kazakhstan Tenge",
            "LAK Laos Kip",
            "LBP Lebanon Pound",
            "LKR Sri Lanka Rupee",
            "LRD Liberia Dollar",
            "LSL Lesotho Loti",
            "LYD Libya Dinar",
            "MAD Morocco Dirham",
            "MDL Moldova Leu",
            "MGA Madagascar Ariary",
            "MKD Macedonia Denar",
            "MMK Myanmar (Burma) Kyat",
            "MNT Mongolia Tughrik",
            "MOP Macau Pataca",
            "MRO Mauritania Ouguiya",
            "MUR Mauritius Rupee",
            "MVR Maldives (Maldive Islands) Rufiyaa",
            "MWK Malawi Kwacha",
            "MXN Mexico Peso",
            "MYR Malaysia Ringgit",
            "MZN Mozambique Metical",
            "NAD Namibia Dollar",
            "NGN Nigeria Naira",
            "NIO Nicaragua Cordoba",
            "NOK Norway Krone",
            "NPR Nepal Rupee",
            "NZD New Zealand Dollar",
            "OMR Oman Rial",
            "PAB Panama Balboa",
            "PEN Peru Sol",
            "PGK Papua New Guinea Kina",
            "PHP Philippines Peso",
            "PKR Pakistan Rupee",
            "PLN Poland Zloty",
            "PYG Paraguay Guarani",
            "QAR Qatar Riyal",
            "RON Romania New Leu",
            "RSD Serbia Dinar",
            "RUB Russia Ruble",
            "RWF Rwanda Franc",
            "SAR Saudi Arabia Riyal",
            "SBD Solomon Islands Dollar",
            "SCR Seychelles Rupee",
            "SDG Sudan Pound",
            "SEK Sweden Krona",
            "SGD Singapore Dollar",
            "SHP Saint Helena Pound",
            "SLL Sierra Leone Leone",
            "SOS Somalia Shilling",
            "SPL Seborga Luigino",
            "SRD Suriname Dollar",
            "STD São Tomé and Príncipe Dobra",
            "SVC El Salvador Colon",
            "SYP Syria Pound",
            "SZL Swaziland Lilangeni",
            "THB Thailand Baht",
            "TJS Tajikistan Somoni",
            "TMT Turkmenistan Manat",
            "TND Tunisia Dinar",
            "TOP Tonga Pa'anga",
            "TRY Turkey Lira",
            "TTD Trinidad and Tobago Dollar",
            "TVD Tuvalu Dollar",
            "TWD Taiwan New Dollar",
            "TZS Tanzania Shilling",
            "UAH Ukraine Hryvnia",
            "UGX Uganda Shilling",
            "USD United States Dollar",
            "UYU Uruguay Peso",
            "UZS Uzbekistan Som",
            "VEF Venezuela Bolivar",
            "VND Viet Nam Dong",
            "VUV Vanuatu Vatu",
            "WST Samoa Tala",
            "XAF Communauté Financière Africaine (BEAC) CFA Franc BEAC",
            "XCD East Caribbean Dollar",
            "XDR International Monetary Fund (IMF) Special Drawing Rights",
            "XOF Communauté Financière Africaine (BCEAO) Franc",
            "XPF Comptoirs Français du Pacifique (CFP) Franc",
            "YER Yemen Rial",
            "ZAR South Africa Rand",
            "ZMW Zambia Kwacha",
            "ZWD Zimbabwe Dollar"
        };
        #endregion
        private static Lazy<Dictionary<string, string>> _currencyCodes =
            new Lazy<Dictionary<string, string>>(delegate {
                Dictionary<string, string> currencyCodes = new Dictionary<string, string>();
                //the array above has code/description pairs, each in a string, like so:
                // "CCC Description"
                //The first 3 characters are the code
                //then there is a single space
                //then the description
                foreach (string line in ISO4217) {
                    string code = line.Substring(0, 3);
                    string desc = line.Substring(4, line.Length - 4);
                    currencyCodes.Add(code, desc);
                }
                return currencyCodes;
            });


        public static Dictionary<string, string> CurrencyCodes { get { return _currencyCodes.Value; } }

        public static readonly Dictionary<string, Currency> Currencies = new Dictionary<string, Currency>(InitializeIsoCurrencies());

        private static readonly string CurrencySign = CultureInfo.InvariantCulture.NumberFormat.CurrencySymbol;

        private static IDictionary<string, Currency> InitializeIsoCurrencies() {
            return new Dictionary<string, Currency> {
                { "AED", new Currency("AED", "United Arab Emirates dirham", "د.إ") },
                { "AFN", new Currency("AFN", "Afghan afghani", "؋") },
                { "ALL", new Currency("ALL", "Albanian lek", "Lek") },
                { "AMD", new Currency("AMD", "Armenian dram", "֏") },
                { "ANG", new Currency("ANG", "Netherlands Antillean guilder", "ƒ") },
                { "AOA", new Currency("AOA", "Angolan kwanza", "Kz") },
                { "ARS", new Currency("ARS", "Argentine peso", "$") },
                { "AUD", new Currency("AUD", "Australian dollar", "$") },
                { "AWG", new Currency("AWG", "Aruban florin", "ƒ") },
                { "AZN", new Currency("AZN", "Azerbaijani manat", "ман") },
                { "BAM", new Currency("BAM", "Bosnia and Herzegovina convertible mark", "KM") },
                { "BBD", new Currency("BBD", "Barbados dollar", "$") },
                { "BDT", new Currency("BDT", "Bangladeshi taka", "৳") },
                { "BGN", new Currency("BGN", "Bulgarian lev", "лв.") },
                { "BHD", new Currency("BHD", "Bahraini dinar", "د.ب.") },
                { "BIF", new Currency("BIF", "Burundian franc", "FBu") },
                { "BMD", new Currency("BMD", "Bermudian dollar", "$") },
                { "BND", new Currency("BND", "Brunei dollar", "$") },
                { "BOB", new Currency("BOB", "Boliviano", "Bs.") },
                { "BOV", new Currency("BOV", "Bolivian Mvdol (funds code)", Currency.CurrencySign) },
                { "BRL", new Currency("BRL", "Brazilian real", "R$") },
                { "BSD", new Currency("BSD", "Bahamian dollar", "$") },
                { "BTN", new Currency("BTN", "Bhutanese ngultrum", "Nu.") },
                { "BWP", new Currency("BWP", "Botswana pula", "P") },
                { "BYN", new Currency("BYN", "Belarusian ruble", "Br") },
                { "BZD", new Currency("BZD", "Belize dollar", "BZ$") },
                { "CAD", new Currency("CAD", "Canadian dollar", "$") },
                { "CDF", new Currency("CDF", "Congolese franc", "FC") },
                { "CHE", new Currency("CHE", "WIR Euro (complementary currency)", "CHE") },
                { "CHF", new Currency("CHF", "Swiss franc", "CHF") },
                { "CHW", new Currency("CHW", "WIR Franc (complementary currency)", "CHW") },
                { "CLF", new Currency("CLF", "Unidad de Fomento (funds code)", "CLF") },
                { "CLP", new Currency("CLP", "Chilean peso", "$") },
                { "CNY", new Currency("CNY", "Yuan Renminbi", "¥") },
                { "COP", new Currency("COP", "Colombian peso", "$") },
                { "COU", new Currency("COU", "Unidad de Valor Real", Currency.CurrencySign) },
                { "CRC", new Currency("CRC", "Costa Rican colon", "₡") },
                { "CUC", new Currency("CUC", "Cuban convertible peso", "CUC$") },
                { "CUP", new Currency("CUP", "Cuban peso", "$") },
                { "CVE", new Currency("CVE", "Cape Verde escudo", "$") },
                { "CZK", new Currency("CZK", "Czech koruna", "Kč") },
                { "DJF", new Currency("DJF", "Djiboutian franc", "Fdj") },
                { "DKK", new Currency("DKK", "Danish krone", "kr.") },
                { "DOP", new Currency("DOP", "Dominican peso", "RD$") },
                { "DZD", new Currency("DZD", "Algerian dinar", "DA") }, 
                { "EGP", new Currency("EGP", "Egyptian pound", "E£") },
                { "ERN", new Currency("ERN", "Eritrean nakfa", "ERN") },
                { "ETB", new Currency("ETB", "Ethiopian birr", "Br") },
                { "EUR", new Currency("EUR", "Euro", "€") },
                { "FJD", new Currency("FJD", "Fiji dollar", "FJ$") },
                { "FKP", new Currency("FKP", "Falkland Islands pound", "£") },
                { "GBP", new Currency("GBP", "Pound sterling", "£") },
                { "GEL", new Currency("GEL", "Georgian lari", "ლ") },
                { "GHS", new Currency("GHS", "Ghanaian cedi", "GH¢") },
                { "GIP", new Currency("GIP", "Gibraltar pound", "£") },
                { "GMD", new Currency("GMD", "Gambian dalasi", "D") },
                { "GNF", new Currency("GNF", "Guinean franc", "FG") },
                { "GTQ", new Currency("GTQ", "Guatemalan quetzal", "Q") },
                { "GYD", new Currency("GYD", "Guyanese dollar", "G$") },
                { "HKD", new Currency("HKD", "Hong Kong dollar", "HK$") },
                { "HNL", new Currency("HNL", "Honduran lempira", "L") },
                { "HRK", new Currency("HRK", "Croatian kuna", "kn") },
                { "HTG", new Currency("HTG", "Haitian gourde", "G") },
                { "HUF", new Currency("HUF", "Hungarian forint", "Ft") },
                { "IDR", new Currency("IDR", "Indonesian rupiah", "Rp") },
                { "ILS", new Currency("ILS", "Israeli new shekel", "₪") },
                { "INR", new Currency("INR", "Indian rupee", "₹") },
                { "IQD", new Currency("IQD", "Iraqi dinar", "د.ع") },
                { "IRR", new Currency("IRR", "Iranian rial", "ريال") },
                { "ISK", new Currency("ISK", "Icelandic króna", "kr") },
                { "JMD", new Currency("JMD", "Jamaican dollar", "J$") },
                { "JOD", new Currency("JOD", "Jordanian dinar", "د.ا.‏") },
                { "JPY", new Currency("JPY", "Japanese yen", "¥") },
                { "KES", new Currency("KES", "Kenyan shilling", "KSh") },
                { "KGS", new Currency("KGS", "Kyrgyzstani som", "сом") },
                { "KHR", new Currency("KHR", "Cambodian riel", "៛") },
                { "KMF", new Currency("KMF", "Comoro franc", "CF") },
                { "KPW", new Currency("KPW", "North Korean won", "₩") },
                { "KRW", new Currency("KRW", "South Korean won", "₩") },
                { "KWD", new Currency("KWD", "Kuwaiti dinar", "د.ك") },
                { "KYD", new Currency("KYD", "Cayman Islands dollar", "$") },
                { "KZT", new Currency("KZT", "Kazakhstani tenge", "₸") },
                { "LAK", new Currency("LAK", "Lao kip", "₭") },
                { "LBP", new Currency("LBP", "Lebanese pound", "ل.ل") },
                { "LKR", new Currency("LKR", "Sri Lankan rupee", "Rs") },
                { "LRD", new Currency("LRD", "Liberian dollar", "L$") },
                { "LSL", new Currency("LSL", "Lesotho loti", "L") }, 
                { "LYD", new Currency("LYD", "Libyan dinar", "ل.د") },
                { "MAD", new Currency("MAD", "Moroccan dirham", "د.م.") },
                { "MDL", new Currency("MDL", "Moldovan leu", "L") },
                { "MGA", new Currency("MGA", "Malagasy ariary", "Ar") },
                { "MKD", new Currency("MKD", "Macedonian denar", "ден") },
                { "MMK", new Currency("MMK", "Myanma kyat", "K") },
                { "MNT", new Currency("MNT", "Mongolian tugrik", "₮") },
                { "MOP", new Currency("MOP", "Macanese pataca", "MOP$") },
                { "MRO", new Currency("MRO", "Mauritanian ouguiya", "UM") },
                { "MUR", new Currency("MUR", "Mauritian rupee", "Rs") },
                { "MVR", new Currency("MVR", "Maldivian rufiyaa", "Rf") },
                { "MWK", new Currency("MWK", "Malawi kwacha", "MK") },
                { "MXN", new Currency("MXN", "Mexican peso", "$") },
                { "MXV", new Currency("MXV", "Mexican Unidad de Inversion (UDI) (funds code)", Currency.CurrencySign) },
                { "MYR", new Currency("MYR", "Malaysian ringgit", "RM") },
                { "MZN", new Currency("MZN", "Mozambican metical", "MTn") },
                { "NAD", new Currency("NAD", "Namibian dollar", "N$") }, 
                { "NGN", new Currency("NGN", "Nigerian naira", "₦") },
                { "NIO", new Currency("NIO", "Nicaraguan córdoba oro", "C$") },
                { "NOK", new Currency("NOK", "Norwegian krone", "kr") },
                { "NPR", new Currency("NPR", "Nepalese rupee", "Rs") },
                { "NZD", new Currency("NZD", "New Zealand dollar", "$") },
                { "OMR", new Currency("OMR", "Omani rial", "ر.ع.") },
                { "PAB", new Currency("PAB", "Panamanian balboa", "B/.") },
                { "PEN", new Currency("PEN", "Peruvian sol", "S/.") },
                { "PGK", new Currency("PGK", "Papua New Guinean kina", "K") },
                { "PHP", new Currency("PHP", "Philippine peso", "₱") },
                { "PKR", new Currency("PKR", "Pakistani rupee", "Rs") },
                { "PLN", new Currency("PLN", "Polish złoty", "zł") },
                { "PYG", new Currency("PYG", "Paraguayan guaraní", "₲") },
                { "QAR", new Currency("QAR", "Qatari riyal", "ر.ق") }, 
                { "RON", new Currency("RON", "Romanian new leu", "lei") },
                { "RSD", new Currency("RSD", "Serbian dinar", "РСД") },
                { "RUB", new Currency("RUB", "Russian rouble", "₽") },
                { "RWF", new Currency("RWF", "Rwandan franc", "RFw") },
                { "SAR", new Currency("SAR", "Saudi riyal", "ر.س") },
                { "SBD", new Currency("SBD", "Solomon Islands dollar", "SI$") },
                { "SCR", new Currency("SCR", "Seychelles rupee", "SR") },
                { "SDG", new Currency("SDG", "Sudanese pound", "ج.س.") },
                { "SEK", new Currency("SEK", "Swedish krona/kronor", "kr") },
                { "SGD", new Currency("SGD", "Singapore dollar", "S$") },
                { "SHP", new Currency("SHP", "Saint Helena pound", "£") },
                { "SLL", new Currency("SLL", "Sierra Leonean leone", "Le") },
                { "SOS", new Currency("SOS", "Somali shilling", "S") },
                { "SRD", new Currency("SRD", "Surinamese dollar", "$") },
                { "SSP", new Currency("SSP", "South Sudanese pound", "£") }, 
                { "STD", new Currency("STD", "São Tomé and Príncipe dobra", "Db") },
                { "SVC", new Currency("SVC", "El Salvador Colon", "₡") },
                { "SYP", new Currency("SYP", "Syrian pound", "ܠ.ܣ.‏") },
                { "SZL", new Currency("SZL", "Swazi lilangeni", "L") },
                { "THB", new Currency("THB", "Thai baht", "฿") },
                { "TJS", new Currency("TJS", "Tajikistani somoni", "смн") },
                { "TMT", new Currency("TMT", "Turkmenistani new manat", "m") }, 
                { "TND", new Currency("TND", "Tunisian dinar", "د.ت") }, 
                { "TOP", new Currency("TOP", "Tongan paʻanga", "T$") },
                { "TRY", new Currency("TRY", "Turkish lira", "₺") },
                { "TTD", new Currency("TTD", "Trinidad and Tobago dollar", "$") },
                { "TWD", new Currency("TWD", "New Taiwan dollar", "NT$") },
                { "TZS", new Currency("TZS", "Tanzanian shilling", "x/y") }, 
                { "UAH", new Currency("UAH", "Ukrainian hryvnia", "₴") },
                { "UGX", new Currency("UGX", "Ugandan shilling", "USh") },
                { "USD", new Currency("USD", "United States dollar", "$") }, 
                { "USN", new Currency("USN", "United States dollar (next day) (funds code)", "$") },
                { "UYI", new Currency("UYI", "Uruguay Peso en Unidades Indexadas (URUIURUI) (funds code)", Currency.CurrencySign) },
                { "UYU", new Currency("UYU", "Uruguayan peso", "$") },
                { "UZS", new Currency("UZS", "Uzbekistan sum", "лв") },
                { "VEF", new Currency("VEF", "Venezuelan bolívar", "Bs.") },
                { "VND", new Currency("VND", "Vietnamese dong", "₫") },
                { "VUV", new Currency("VUV", "Vanuatu vatu", "VT") },
                { "WST", new Currency("WST", "Samoan tala", "WS$") },
                { "XAF", new Currency("XAF", "CFA franc BEAC", "FCFA") },
                { "XCD", new Currency("XCD", "East caribbean dollar", "")},
                { "XOF", new Currency("XOF", "CFA franc BCEAO", "FCFA") },
                { "XPF", new Currency("XPF", "CFP franc", "") },
                { "YER", new Currency("YER", "Yemeni rial", "﷼") },
                { "ZAR", new Currency("ZAR", "South African rand", "R") },
                { "ZMW", new Currency("ZMW", "Zambian kwacha", "ZK") },
                { "ZWL", new Currency("ZWL", "Zimbabwean dollar", "$") }
            };
        }

        /// <summary>
        /// Given a currency code, returns a string representing the currency.
        /// </summary>
        /// <param name="code">The currency code</param>
        /// <returns>A string representing the currency code as symbol, or the same code if no symbol was found.</returns>
        /// <example>Given "EUR", this will return "€".</example>
        public static string GetCurrencySymbol(string code) {
            //http://www.xe.com/symbols.php has a table of symbols
            //TODO: implement symbols from that table
            if (code == "EUR") {
                return "€";
            }
            else if (code == "USD") {
                return "$";
            }
            else {
                return code;
            }
        }

        /// <summary>
        /// Given a currency code, returns a string representing the format to be used to write currency amounts.
        /// </summary>
        /// <param name="code">The currency code</param>
        /// <returns>A string representing the currency format</returns>
        /// <example>Given "EUR", this will return "€".</example>
        public static string GetCurrencyFormat(string code) {
            //http://www.thefinancials.com/?SubSectionID=curformat has a table including formats
            //TODO: implement symbols from that table
            return "0.00";
        }

        public static string GetPriceAsString(double price, string code) {
            return price.ToString(GetCurrencyFormat(code)) + " " + GetCurrencySymbol(code);
        }
        public static string GetPriceAsString(decimal price, string code) {
            return price.ToString(GetCurrencyFormat(code)) + " " + GetCurrencySymbol(code);
        }
        public static string GetPriceAsString(double? price, string code) {
            if (price.HasValue) {
                return GetPriceAsString(price.Value, code);
            }
            return string.Empty;
        }
        public static string GetPriceAsString(decimal? price, string code) {
            if (price.HasValue) {
                return GetPriceAsString(price.Value, code);
            }
            return string.Empty;
        }

        public string CurrencyCode { get; set; } //3-letter ISO 4217 code
        public string CurrencyName { get; set; } //ISO 4217 currency name
        public string Symbol { get; set; } //Symbol used for the currency
        //Properties we use to format strings using the currency:
        private NumberFormatInfo _numberFormatInfo;
        private CultureInfo _cultureInfo;
        //

        /// <summary>
        /// Creates a new instance of Currency
        /// </summary>
        /// <param name="currencyCode">The ISO-4217 code identifying the currency</param>
        /// <param name="currencyName">The name of the currency</param>
        /// <param name="symbol">The symbol to be used for the currency</param>
        /// <param name="decimalDigits">The number of decimal places to use</param>
        /// <param name="decimalSeparator">The string to use as the decimal separator</param>
        /// <param name="groupSeparator">The string that separates groups of digits to the left of the decimal</param>
        /// <param name="groupSizes">The number of digits in each group to the left of the decimal</param>
        public Currency(string currencyCode, string currencyName, string symbol,
            int decimalDigits = -1, string decimalSeparator = null, string groupSeparator = null, IEnumerable<int> groupSizes = null) {

            CurrencyCode = currencyCode;
            CurrencyName = currencyName;
            Symbol = symbol;
            //generate this Currency's NumberFormatInfo

            //generate this Currency's CultureInfo
        }
    }
}
