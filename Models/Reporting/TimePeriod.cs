using System;
using System.Globalization;

namespace Nwazet.Commerce.Models.Reporting {
    public struct TimePeriod {
        private readonly int _value;

        private TimePeriod(int value) {
            _value = value;
        }

        public static TimePeriod Hour = new TimePeriod(0);
        public static TimePeriod Day = new TimePeriod(1);
        public static TimePeriod Week = new TimePeriod(2);
        public static TimePeriod Month = new TimePeriod(3);
        public static TimePeriod Trimester = new TimePeriod(4);
        public static TimePeriod Year = new TimePeriod(5);

        public DateTime BeginningDate(DateTime date, CultureInfo cultureInfo = null) {
            switch (_value) {
                case 0: // hour
                    return date.Date.AddHours(date.Hour);
                case 1: // day
                    return date.Date;
                case 2: // week
                    var firstDayOfWeek = 
                        cultureInfo != null
                        ? cultureInfo.DateTimeFormat.FirstDayOfWeek
                        : DateTimeFormatInfo.CurrentInfo == null
                        ? DayOfWeek.Sunday
                        : DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek;
                    var distanceToMonday = date.DayOfWeek - firstDayOfWeek;
                    if (distanceToMonday < 0) {
                        distanceToMonday += 7;
                    }
                    return date.Date.AddDays(-distanceToMonday);
                case 3: // month
                    return date.Date.AddDays(1-date.Day);
                case 4: // trimester
                    var distanceToTrimesterStartMonth = (date.Month - 1)%3;
                    return date.Date.AddMonths(-distanceToTrimesterStartMonth).AddDays(1-date.Day);
                case 5: //year
                    return date.Date.AddDays(1-date.DayOfYear);
            }
            throw new InvalidOperationException("Unknown time period");
        }

        public DateTime EndingDate(DateTime date, CultureInfo cultureInfo = null) {
            return BeginningDate(date, cultureInfo) + this;
        }

        public int PeriodCount(DateTime startDate, DateTime endDate, CultureInfo cultureInfo = null) {
            startDate = BeginningDate(startDate, cultureInfo);
            endDate = EndingDate(endDate, cultureInfo);
            switch (_value) {
                case 0: // hour
                    return (int) ((endDate.Ticks - startDate.Ticks)/TimeSpan.TicksPerHour);
                case 1: // day
                    return (int) ((endDate.Ticks - startDate.Ticks)/TimeSpan.TicksPerDay);
                case 2: // week
                    return (int) ((endDate.Ticks - startDate.Ticks)/TimeSpan.TicksPerDay/7);
                case 3: // month
                    return (endDate.Year*12 + endDate.Month)
                           - (startDate.Year*12 + startDate.Month);
                case 4: // trimester
                    return ((endDate.Year*12 + endDate.Month)
                            - (startDate.Year*12 + startDate.Month))/3;
                case 5: // year
                    return endDate.Year - startDate.Year;
            }
            throw new InvalidOperationException("Unknown time period");
        }

        public static TimePeriod Parse(string value) {
            switch (value.ToLowerInvariant()) {
                case "hour":
                    return Hour;
                case "day":
                    return Day;
                case "week":
                    return Week;
                case "month":
                    return Month;
                case "trimester":
                    return Trimester;
                case "year":
                    return Year;
            }
            throw new InvalidOperationException("Unknown time period");
        }

        public static DateTime operator +(DateTime date, TimePeriod period) {
            switch (period._value) {
                case 0: // hour
                    return date.AddHours(1);
                case 1: // day
                    return date.AddDays(1);
                case 2: // week
                    return date.AddDays(7);
                case 3: // month
                    return date.AddMonths(1);
                case 4: // trimester
                    return date.AddMonths(3);
                case 5: // year
                    return date.AddYears(1);
            }
            throw new InvalidOperationException("Unknown time period");
        }

        public static DateTime operator -(DateTime date, TimePeriod period) {
            switch (period._value) {
                case 0: // hour
                    return date.AddHours(-1);
                case 1: // day
                    return date.AddDays(-1);
                case 2: // week
                    return date.AddDays(-7);
                case 3: //month
                    return date.AddMonths(-1);
                case 4: // trimester
                    return date.AddMonths(-3);
                case 5: // year
                    return date.AddYears(-1);
            }
            throw new InvalidOperationException("Unknown time period");
        }

        public static bool operator ==(TimePeriod a, TimePeriod b) {
            return a._value == b._value;
        }

        public static bool operator !=(TimePeriod a, TimePeriod b) {
            return a._value != b._value;
        }

        public override bool Equals(object obj) {
            if (!(obj is TimePeriod)) return false;
            return ((TimePeriod) obj)._value == _value;
        }

        public override int GetHashCode() {
            return _value.GetHashCode();
        }

        public override string ToString() {
            switch (_value) {
                case 0:
                    return "Hour";
                case 1:
                    return "Day";
                case 2:
                    return "Week";
                case 3:
                    return "Month";
                case 4:
                    return "Trimester";
                case 5:
                    return "Year";
            }
            throw new InvalidOperationException("Unknown time period");
        }

        public string ToString(DateTime date, CultureInfo culture = null) {
            if (culture == null) {
                culture = CultureInfo.CurrentUICulture;
            }
            switch (_value) {
                case 0: // hour
                    return BeginningDate(date, culture)
                        .ToString(date.Hour == 0 ? "g" : "t", culture);
                case 1: // day
                    return date.ToString("d", culture);
                case 2: // week
                    var startDate = BeginningDate(date, culture);
                    var endDate = startDate.AddDays(6);
                    return String.Format(
                        culture,
                        startDate.Year == endDate.Year
                            ? startDate.Month == endDate.Month
                                ? "{0:%d} - {1:%d}"
                                : "{0:m} - {1:m}"
                            : "{0:d} - {1:d}"
                        , startDate, endDate);
                case 3: //month
                    return date.ToString(date.Month == 1 ? "y" : "MMMM", culture);
                case 4: // trimester
                    var trimester = (date.Month - 1)/3 + 1;
                    return String.Format(
                        culture,
                        trimester == 1 ? "{0:yyyy} Q{1}" : "Q{1}",
                        date,
                        trimester);
                case 5: // year
                    return date.ToString("yyyy", culture);
            }
            throw new InvalidOperationException("Unknown time period");
        }
    }
}