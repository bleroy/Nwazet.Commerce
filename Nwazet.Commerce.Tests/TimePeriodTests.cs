using System;
using System.Globalization;
using NUnit.Framework;
using Nwazet.Commerce.Models.Reporting;

namespace Nwazet.Commerce.Tests {
    [TestFixture]
    public class TimePeriodTests {
        [Test]
        public void BeginningDatePointsToStartOfPeriod() {
            var date = new DateTime(1970, 5, 21, 12, 32, 54);

            Assert.That(
                TimePeriod.Hour.BeginningDate(date),
                Is.EqualTo(new DateTime(1970, 5, 21, 12, 00, 00)));
            
            Assert.That(
                TimePeriod.Day.BeginningDate(date),
                Is.EqualTo(new DateTime(1970, 5, 21, 00, 00, 00)));
            
            var firstDayOfPeriod = TimePeriod.Week.BeginningDate(date);
            Assert.That(
                (date - firstDayOfPeriod).Days,
                Is.LessThan(7).And.AtLeast(0));
            var firstDayOfWeek = DateTimeFormatInfo.CurrentInfo == null
                ? DayOfWeek.Sunday
                : DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek;
            Assert.That(
                firstDayOfPeriod.DayOfWeek,
                Is.EqualTo(firstDayOfWeek));

            Assert.That(
                TimePeriod.Month.BeginningDate(date),
                Is.EqualTo(new DateTime(1970, 5, 1, 00, 00, 00)));
            
            Assert.That(
                TimePeriod.Trimester.BeginningDate(date),
                Is.EqualTo(new DateTime(1970, 4, 1, 00, 00, 00)));
            
            Assert.That(
                TimePeriod.Year.BeginningDate(date),
                Is.EqualTo(new DateTime(1970, 1, 1, 00, 00, 00)));
        }

        [Test]
        public void EndingDatePointsToEndOfPeriod() {
            var date = new DateTime(1970, 5, 21, 12, 32, 54);

            Assert.That(
                TimePeriod.Hour.EndingDate(date),
                Is.EqualTo(new DateTime(1970, 5, 21, 13, 00, 00)));

            Assert.That(
                TimePeriod.Day.EndingDate(date),
                Is.EqualTo(new DateTime(1970, 5, 22, 00, 00, 00)));

            var firstDayOfNextWeek = TimePeriod.Week.EndingDate(date);
            Assert.That(
                (firstDayOfNextWeek - date).Days,
                Is.LessThan(7).And.AtLeast(0));
            var firstDayOfWeek = DateTimeFormatInfo.CurrentInfo == null
                ? DayOfWeek.Sunday
                : DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek;
            Assert.That(
                firstDayOfNextWeek.DayOfWeek,
                Is.EqualTo(firstDayOfWeek));
            
            Assert.That(
                TimePeriod.Month.EndingDate(date),
                Is.EqualTo(new DateTime(1970, 6, 1, 00, 00, 00)));

            Assert.That(
                TimePeriod.Trimester.EndingDate(date),
                Is.EqualTo(new DateTime(1970, 7, 1, 00, 00, 00)));
            
            Assert.That(
                TimePeriod.Year.EndingDate(date),
                Is.EqualTo(new DateTime(1971, 1, 1, 00, 00, 00)));
        }

        [Test]
        public void PeriodCountGivesHowManyTimePeriodsOverlap() {
            var date1 = new DateTime(2014, 8, 7, 12, 45, 00);
            var date2 = new DateTime(2014, 8, 8, 6, 12, 00);

            Assert.That(
                TimePeriod.Hour.PeriodCount(date1, date2),
                Is.EqualTo(19));

            date2 = new DateTime(2014, 9, 3, 6, 00, 00);
            Assert.That(
                TimePeriod.Day.PeriodCount(date1, date2),
                Is.EqualTo(28));

            Assert.That(
                TimePeriod.Week.PeriodCount(date1, date2),
                Is.EqualTo(5));

            Assert.That(
                TimePeriod.Month.PeriodCount(date1, date2),
                Is.EqualTo(2));

            date2 = new DateTime(2016, 4, 12);
            Assert.That(
                TimePeriod.Trimester.PeriodCount(date1, date2),
                Is.EqualTo(8));

            Assert.That(
                TimePeriod.Year.PeriodCount(date1, date2),
                Is.EqualTo(3));
        }

        [Test]
        public void ParseGivesTimePeriod() {
            Assert.That(
                TimePeriod.Parse("Hour"),
                Is.EqualTo(TimePeriod.Hour));
            Assert.That(
                TimePeriod.Parse("Day"),
                Is.EqualTo(TimePeriod.Day));
            Assert.That(
                TimePeriod.Parse("Week"),
                Is.EqualTo(TimePeriod.Week));
            Assert.That(
                TimePeriod.Parse("Month"),
                Is.EqualTo(TimePeriod.Month));
            Assert.That(
                TimePeriod.Parse("Trimester"),
                Is.EqualTo(TimePeriod.Trimester));
            Assert.That(
                TimePeriod.Parse("Year"),
                Is.EqualTo(TimePeriod.Year));
        }

        [Test]
        public void AddTimePeriod() {
            var date = new DateTime(1970, 5, 21, 12, 43, 23);

            Assert.That(
                date + TimePeriod.Hour,
                Is.EqualTo(date.AddHours(1)));
            Assert.That(
                date + TimePeriod.Day,
                Is.EqualTo(date.AddDays(1)));
            Assert.That(
                date + TimePeriod.Week,
                Is.EqualTo(date.AddDays(7)));
            Assert.That(
                date + TimePeriod.Month,
                Is.EqualTo(date.AddMonths(1)));
            Assert.That(
                date + TimePeriod.Trimester,
                Is.EqualTo(date.AddMonths(3)));
            Assert.That(
                date + TimePeriod.Year,
                Is.EqualTo(date.AddYears(1)));
        }

        [Test]
        public void SubtractTimePeriod() {
            var date = new DateTime(1970, 5, 21, 12, 43, 23);

            Assert.That(
                date - TimePeriod.Hour,
                Is.EqualTo(date.AddHours(-1)));
            Assert.That(
                date - TimePeriod.Day,
                Is.EqualTo(date.AddDays(-1)));
            Assert.That(
                date - TimePeriod.Week,
                Is.EqualTo(date.AddDays(-7)));
            Assert.That(
                date - TimePeriod.Month,
                Is.EqualTo(date.AddMonths(-1)));
            Assert.That(
                date - TimePeriod.Trimester,
                Is.EqualTo(date.AddMonths(-3)));
            Assert.That(
                date - TimePeriod.Year,
                Is.EqualTo(date.AddYears(-1)));
        }

        [Test]
        public void TimePeriodEquality() {
            Assert.IsTrue(TimePeriod.Hour == TimePeriod.Hour);
            Assert.IsTrue(TimePeriod.Day == TimePeriod.Day);
            Assert.IsTrue(TimePeriod.Week == TimePeriod.Week);
            Assert.IsTrue(TimePeriod.Month == TimePeriod.Month);
            Assert.IsTrue(TimePeriod.Trimester == TimePeriod.Trimester);
            Assert.IsTrue(TimePeriod.Year == TimePeriod.Year);
            Assert.IsFalse(TimePeriod.Hour == TimePeriod.Year);
            Assert.IsFalse(TimePeriod.Day == TimePeriod.Trimester);
            Assert.IsFalse(TimePeriod.Week == TimePeriod.Day);
            Assert.IsFalse(TimePeriod.Month == TimePeriod.Hour);
            Assert.IsFalse(TimePeriod.Trimester == TimePeriod.Week);
            Assert.IsFalse(TimePeriod.Year == TimePeriod.Month);
            Assert.IsFalse(TimePeriod.Hour != TimePeriod.Hour);
            Assert.IsFalse(TimePeriod.Day != TimePeriod.Day);
            Assert.IsFalse(TimePeriod.Week != TimePeriod.Week);
            Assert.IsFalse(TimePeriod.Month != TimePeriod.Month);
            Assert.IsFalse(TimePeriod.Trimester != TimePeriod.Trimester);
            Assert.IsFalse(TimePeriod.Year != TimePeriod.Year);
            Assert.IsTrue(TimePeriod.Hour != TimePeriod.Year);
            Assert.IsTrue(TimePeriod.Day != TimePeriod.Trimester);
            Assert.IsTrue(TimePeriod.Week != TimePeriod.Day);
            Assert.IsTrue(TimePeriod.Month != TimePeriod.Hour);
            Assert.IsTrue(TimePeriod.Trimester != TimePeriod.Week);
            Assert.IsTrue(TimePeriod.Year != TimePeriod.Month);
            Assert.IsTrue(TimePeriod.Day.Equals(TimePeriod.Day));
            Assert.IsFalse(TimePeriod.Hour.Equals(null));
            Assert.IsFalse(TimePeriod.Hour.Equals(3));
            Assert.IsFalse(TimePeriod.Hour.Equals(TimePeriod.Year));
        }

        [Test]
        public void ToStringGivesRightString() {
            Assert.That(TimePeriod.Hour.ToString(), Is.EqualTo("Hour"));
            Assert.That(TimePeriod.Day.ToString(), Is.EqualTo("Day"));
            Assert.That(TimePeriod.Week.ToString(), Is.EqualTo("Week"));
            Assert.That(TimePeriod.Month.ToString(), Is.EqualTo("Month"));
            Assert.That(TimePeriod.Trimester.ToString(), Is.EqualTo("Trimester"));
            Assert.That(TimePeriod.Year.ToString(), Is.EqualTo("Year"));
        }

        [Test]
        public void ToStringOnDateDescribesCurrentPeriod() {
            var french = CultureInfo.CreateSpecificCulture("fr-FR");
            var date = new DateTime(1970, 5, 21, 12, 43, 23);

            Assert.That(
                TimePeriod.Hour.ToString(date, french),
                Is.EqualTo("12:00"));
            Assert.That(
                TimePeriod.Hour.ToString(new DateTime(1970, 5, 21, 0, 34, 00), french),
                Is.EqualTo("21/05/1970 00:00"));

            Assert.That(
                TimePeriod.Day.ToString(date, french),
                Is.EqualTo("21/05/1970"));
            
            Assert.That(
                TimePeriod.Week.ToString(date, french),
                Is.EqualTo("18 - 24"));
            Assert.That(
                TimePeriod.Week.ToString(
                    new DateTime(2014, 8, 1), french),
                Is.EqualTo("28 juillet - 3 août"));
            Assert.That(
                TimePeriod.Week.ToString(
                    new DateTime(2014, 1, 1), french),
                Is.EqualTo("30/12/2013 - 05/01/2014"));

            Assert.That(
                TimePeriod.Month.ToString(date, french),
                Is.EqualTo("mai"));

            Assert.That(
                TimePeriod.Trimester.ToString(date, french),
                Is.EqualTo("Q2"));
            Assert.That(
                TimePeriod.Trimester.ToString(
                    new DateTime(2014, 2, 1), french),
                Is.EqualTo("2014 Q1"));

            Assert.That(
                TimePeriod.Year.ToString(date, french),
                Is.EqualTo("1970"));
        }
    }
}
