using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Nwazet.Commerce.Services {
    public static class UnitedStates {
        private static readonly Regex NonDigitEx = new Regex("\\D");

        private static readonly IDictionary<string, string> StateDictionary =
            new Dictionary<string, string> {
                {"AL", "Alabama"},
                {"AK", "Alaska"},
                {"AZ", "Arizona"},
                {"AR", "Arkansas"},
                {"CA", "California"},
                {"CO", "Colorado"},
                {"CT", "Connecticut"},
                {"DE", "Delaware"},
                {"DC", "District of Columbia"},
                {"FL", "Florida"},
                {"GA", "Georgia"},
                {"HI", "Hawaii"},
                {"ID", "Idaho"},
                {"IL", "Illinois"},
                {"IN", "Indiana"},
                {"IA", "Iowa"},
                {"KS", "Kansas"},
                {"KY", "Kentucky"},
                {"LA", "Louisiana"},
                {"ME", "Maine"},
                {"MD", "Maryland"},
                {"MA", "Massachusetts"},
                {"MI", "Michigan"},
                {"MN", "Minnesota"},
                {"MS", "Mississippi"},
                {"MO", "Missouri"},
                {"MT", "Montana"},
                {"NE", "Nebraska"},
                {"NV", "Nevada"},
                {"NH", "New Hampshire"},
                {"NJ", "New Jersey"},
                {"NM", "New Mexico"},
                {"NY", "New York"},
                {"NC", "North Carolina"},
                {"ND", "North Dakota"},
                {"OH", "Ohio"},
                {"OK", "Oklahoma"},
                {"OR", "Oregon"},
                {"PA", "Pennsylvania"},
                {"RI", "Rhode Island"},
                {"SC", "South Carolina"},
                {"SD", "South Dakota"},
                {"TN", "Tennessee"},
                {"TX", "Texas"},
                {"UT", "Utah"},
                {"VT", "Vermont"},
                {"VA", "Virginia"},
                {"WA", "Washington"},
                {"WV", "West Virginia"},
                {"WI", "Wisconsin"},
                {"WY", "Wyoming"},
                {"AS", "American Samoa"},
                {"GU", "Guam"},
                {"MP", "Northern Mariana Islands"},
                {"PR", "Puerto Rico"},
                {"VI", "Virgin Islands"},
                {"FM", "Federated States of Micronesia"},
                {"MH", "Marshall Islands"},
                {"PW", "Palau"},
                {"AA", "U.S. Armed Forces – Americas"},
                {"AE", "U.S. Armed Forces – Europe"},
                {"AP", "U.S. Armed Forces – Pacific"},
                {"CM", "Northern Mariana Islands"},
                {"CZ", "Panama Canal Zone"},
                {"NB", "Nebraska"},
                {"PI", "Philippine Islands"},
                {"TT", "Trust Territory of the Pacific Islands"}
            };

        public static IDictionary<string, string> States {
            get { return StateDictionary; }
        }

        public static string State(string zipCode) {
            var nonDigit = NonDigitEx.Match(zipCode);
            if (nonDigit.Success) {
                zipCode = zipCode.Substring(0, nonDigit.Index);
            }
            var code = int.Parse(zipCode);

            if (code < 37000) {
                if (code < 19700) {
                    if (code < 5000) {
                        if (code < 1000) {
                            if (code < 800) {
                                return code < 600 ? "NY" : "PR";
                            }
                            return code < 900 ? "VI" : "PR";
                        }
                        if (code < 3000) {
                            return code < 2800 ? "MA" : "RI";
                        }
                        return code < 3900 ? "NH" : "ME";
                    }
                    if (code < 7000) {
                        if (code < 6000) {
                            return code == 5501 || code == 5544 ? "MA" : "VT";
                        }
                        return code == 6390 ? "NY" : "CT";
                    }
                    if (code < 10000) {
                        return code < 9000 ? "NJ" : "AE";
                    }
                    return code < 15000 ? "NY" : "PA";
                }
                if (code < 24700) {
                    if (code == 20588) return "MD";
                    if (code == 20598) return "VA";
                    if (code < 20600) {
                        if (code < 20100) {
                            return code < 20000 ? "DE" : "DC";
                        }
                        return code < 20200 ? "VA" : "DC";
                    }
                    return code < 22000 ? "MD" : "VA";
                }
                if (code < 32000) {
                    if (code < 29000) {
                        return code < 27000 ? "WV" : "NC";
                    }
                    return code < 30000 ? "SC" : "GA";
                }
                if (code < 34100) {
                    return code < 34000 ? "FL" : "AA";
                }
                return code < 35000 ? "FL" : "AL";
            }
            if (code < 66000) {
                if (code < 53000) {
                    if (code < 43000) {
                        if (code < 39800) {
                            return code < 38600 ? "TN" : "MS";
                        }
                        return code < 40000 ? "GA" : "KY";
                    }
                    if (code < 48000) {
                        return code < 46000 ? "OH" : "IN";
                    }
                    return code < 50000 ? "MI" : "IA";
                }
                if (code < 58000) {
                    if (code < 56900) {
                        return code < 55000 ? "WI" : "MN";
                    }
                    return code < 57000 ? "DC" : "SD";
                }
                if (code < 60000) {
                    return code < 59000 ? "ND" : "MT";
                }
                return code < 63000 ? "IL" : "MO";
            }
            if (code < 82000) {
                if (code < 73000) {
                    if (code < 70000) {
                        return code < 68000 ? "KS" : "NE";
                    }
                    return code < 71600 ? "LA" : "AR";
                }
                if (code < 75000) {
                    return code == 73301 || code == 73344 ? "TX" : "OK";
                }
                return code < 80000 ? "TX" : "CO";
            }
            if (code < 85000) {
                if (code == 83414) return "WY";
                return code < 84000 ? code < 83200 ? "WY" : "ID" : "UT";
            }
            if (code < 88500) {
                return code < 87000 ? "AZ" : "NM";
            }
            if (code < 90000) {
                return code < 88900 ? "TX" : "NV";
            }
            if (code < 96960) {
                if (code < 96900) {
                    if (code < 96700) {
                        return code < 96200 ? "CA" : "AP";
                    }
                    return code == 96799 ? "AS" : "HI";
                }
                if (code < 96941) {
                    return code < 96939 ? "GU" : "PW";
                }
                return code < 96950 ? "FM" : "MP";
            }
            if (code < 98000) {
                return code < 97000 ? "MH" : "OR";
            }
            if (code < 99999) {
                return code < 99500 ? "WA" : "AK";
            }
            return null;
        }
    }
}