using System;
using System.Xml.Linq;
using NUnit.Framework;
using Nwazet.Commerce.Helpers;

namespace Nwazet.Commerce.Tests {
    [TestFixture]
    public class XmlHelperTests {
        [Test]
        public void StringToAttribute() {
            var el = new XElement("data");
            el.Attr("foo", "bar");

            Assert.That(el.Attribute("foo").Value, Is.EqualTo("bar"));
        }

        [Test]
        public void IntToAttribute() {
            var el = new XElement("data");
            el.Attr("foo", 42);

            Assert.That(el.Attribute("foo").Value, Is.EqualTo("42"));
        }

        [Test]
        public void BoolToAttribute() {
            var el = new XElement("data");
            el.Attr("foo", true);
            el.Attr("bar", false);

            Assert.That(el.Attribute("foo").Value, Is.EqualTo("true"));
            Assert.That(el.Attribute("bar").Value, Is.EqualTo("false"));
        }

        [Test]
        public void DateTimeToAttribute() {
            var el = new XElement("data");
            el.Attr("foo", new DateTime(1970, 5, 21, 13, 55, 21, 934, DateTimeKind.Utc));

            Assert.That(el.Attribute("foo").Value, Is.EqualTo("1970-05-21T13:55:21.934Z"));
        }

        [Test]
        public void DoubleFloatDecimalToAttribute() {
            var el = new XElement("data");
            el.Attr("double", 12.456D);
            el.Attr("float", 12.457F);
            el.Attr("decimal", 12.458M);

            Assert.That(el.Attribute("double").Value, Is.EqualTo("12.456"));
            Assert.That(el.Attribute("float").Value, Is.EqualTo("12.457"));
            Assert.That(el.Attribute("decimal").Value, Is.EqualTo("12.458"));
        }

        [Test]
        public void ReadAttribute() {
            var el = XElement.Parse("<data foo=\"bar\"/>");

            Assert.That(el.Attr("foo"), Is.EqualTo("bar"));
            Assert.That(el.Attr("bar"), Is.Null);
        }

        [Test]
        public void StringToElement()
        {
            var el = new XElement("data");
            el.El("foo", "bar");

            Assert.That(el.Element("foo").Value, Is.EqualTo("bar"));
        }

        [Test]
        public void IntToElement()
        {
            var el = new XElement("data");
            el.El("foo", 42);

            Assert.That(el.Element("foo").Value, Is.EqualTo("42"));
        }

        [Test]
        public void BoolToElement()
        {
            var el = new XElement("data");
            el.El("foo", true);
            el.El("bar", false);

            Assert.That(el.Element("foo").Value, Is.EqualTo("true"));
            Assert.That(el.Element("bar").Value, Is.EqualTo("false"));
        }

        [Test]
        public void DateTimeToElement()
        {
            var el = new XElement("data");
            el.El("foo", new DateTime(1970, 5, 21, 13, 55, 21, 934, DateTimeKind.Utc));

            Assert.That(el.Element("foo").Value, Is.EqualTo("1970-05-21T13:55:21.934Z"));
        }

        [Test]
        public void DoubleFloatDecimalToElement()
        {
            var el = new XElement("data");
            el.El("double", 12.456D);
            el.El("float", 12.457F);
            el.El("decimal", 12.458M);

            Assert.That(el.Element("double").Value, Is.EqualTo("12.456"));
            Assert.That(el.Element("float").Value, Is.EqualTo("12.457"));
            Assert.That(el.Element("decimal").Value, Is.EqualTo("12.458"));
        }

        [Test]
        public void ReadElement() {
            var el = XElement.Parse("<data><foo>bar</foo></data>");

            Assert.That(el.El("foo"), Is.EqualTo("bar"));
            Assert.That(el.El("bar"), Is.Null);
        }

        [Test]
        public void SerializeObject() {
            var target = new Target {
                AString = "foo",
                AnInt = 42,
                ABoolean = true,
                ADate = new DateTime(1970, 5, 21, 13, 55, 21, 934, DateTimeKind.Utc),
                ADouble = 12.345D,
                AFloat = 23.456F,
                ADecimal = 34.567M,
                ANullableInt = 42,
                ANullableBoolean = true,
                ANullableDate = new DateTime(1970, 5, 21, 13, 55, 21, 934, DateTimeKind.Utc),
                ANullableDouble = 12.345D,
                ANullableFloat = 23.456F,
                ANullableDecimal = 34.567M
            };
            var el = new XElement("data");
            el.With(target)
              .ToAttr(t => t.AString)
              .ToAttr(t => t.AnInt)
              .ToAttr(t => t.ABoolean)
              .ToAttr(t => t.ADate)
              .ToAttr(t => t.ADouble)
              .ToAttr(t => t.AFloat)
              .ToAttr(t => t.ADecimal)
              .ToAttr(t => t.ANullableInt)
              .ToAttr(t => t.ANullableBoolean)
              .ToAttr(t => t.ANullableDate)
              .ToAttr(t => t.ANullableDouble)
              .ToAttr(t => t.ANullableFloat)
              .ToAttr(t => t.ANullableDecimal);


            Assert.That(el.Attr("AString"), Is.EqualTo("foo"));
            Assert.That(el.Attr("AnInt"), Is.EqualTo("42"));
            Assert.That(el.Attr("ABoolean"), Is.EqualTo("true"));
            Assert.That(el.Attr("ADate"), Is.EqualTo("1970-05-21T13:55:21.934Z"));
            Assert.That(el.Attr("ADouble"), Is.EqualTo("12.345"));
            Assert.That(el.Attr("AFloat"), Is.EqualTo("23.456"));
            Assert.That(el.Attr("ADecimal"), Is.EqualTo("34.567"));
            Assert.That(el.Attr("ANullableInt"), Is.EqualTo("42"));
            Assert.That(el.Attr("ANullableBoolean"), Is.EqualTo("true"));
            Assert.That(el.Attr("ANullableDate"), Is.EqualTo("1970-05-21T13:55:21.934Z"));
            Assert.That(el.Attr("ANullableDouble"), Is.EqualTo("12.345"));
            Assert.That(el.Attr("ANullableFloat"), Is.EqualTo("23.456"));
            Assert.That(el.Attr("ANullableDecimal"), Is.EqualTo("34.567"));
        }

        [Test]
        public void DeSerializeObject() {
            var target = new Target();
            var el =
                XElement.Parse(
                    "<data AString=\"foo\" AnInt=\"42\" ABoolean=\"true\" " +
                    "ADate=\"1970-05-21T13:55:21.934Z\" ADouble=\"12.345\" " +
                    "AFloat=\"23.456\" ADecimal=\"34.567\" " +
                    "ANullableInt=\"42\" ANullableBoolean=\"true\" " +
                    "ANullableDate=\"1970-05-21T13:55:21.934Z\" ANullableDouble=\"12.345\" " +
                    "ANullableFloat=\"23.456\" ANullableDecimal=\"34.567\"/>");
            el.With(target)
              .FromAttr(t => t.AString)
              .FromAttr(t => t.AnInt)
              .FromAttr(t => t.ABoolean)
              .FromAttr(t => t.ADate)
              .FromAttr(t => t.ADouble)
              .FromAttr(t => t.AFloat)
              .FromAttr(t => t.ADecimal)
              .FromAttr(t => t.ANullableInt)
              .FromAttr(t => t.ANullableBoolean)
              .FromAttr(t => t.ANullableDate)
              .FromAttr(t => t.ANullableDouble)
              .FromAttr(t => t.ANullableFloat)
              .FromAttr(t => t.ANullableDecimal);

            Assert.That(target.AString, Is.EqualTo("foo"));
            Assert.That(target.AnInt, Is.EqualTo(42));
            Assert.That(target.ABoolean, Is.True);
            Assert.That(target.ADate, Is.EqualTo(new DateTime(1970, 5, 21, 13, 55, 21, 934, DateTimeKind.Utc)));
            Assert.That(target.ADouble, Is.EqualTo(12.345D));
            Assert.That(target.AFloat, Is.EqualTo(23.456F));
            Assert.That(target.ADecimal, Is.EqualTo(34.567M));
            Assert.That(target.ANullableInt, Is.EqualTo(42));
            Assert.That(target.ANullableBoolean, Is.True);
            Assert.That(target.ANullableDate, Is.EqualTo(new DateTime(1970, 5, 21, 13, 55, 21, 934, DateTimeKind.Utc)));
            Assert.That(target.ANullableDouble, Is.EqualTo(12.345D));
            Assert.That(target.ANullableFloat, Is.EqualTo(23.456F));
            Assert.That(target.ANullableDecimal, Is.EqualTo(34.567M));
        }

        [Test]
        public void DeSerializeFromMissingAttributeLeavesValueIntact() {
            var target = new Target {
                AString = "foo",
                AnInt = 42,
                ABoolean = true,
                ADate = new DateTime(1970, 5, 21, 13, 55, 21, 934, DateTimeKind.Utc),
                ADouble = 12.345D,
                AFloat = 23.456F,
                ADecimal = 34.567M,
                ANullableInt = 42,
                ANullableBoolean = true,
                ANullableDate = new DateTime(1970, 5, 21, 13, 55, 21, 934, DateTimeKind.Utc),
                ANullableDouble = 12.345D,
                ANullableFloat = 23.456F,
                ANullableDecimal = 34.567M
            };
            var el = new XElement("data");
            el.With(target)
              .FromAttr(t => t.AString)
              .FromAttr(t => t.AnInt)
              .FromAttr(t => t.ABoolean)
              .FromAttr(t => t.ADate)
              .FromAttr(t => t.ADouble)
              .FromAttr(t => t.AFloat)
              .FromAttr(t => t.ADecimal)
              .FromAttr(t => t.ANullableInt)
              .FromAttr(t => t.ANullableBoolean)
              .FromAttr(t => t.ANullableDate)
              .FromAttr(t => t.ANullableDouble)
              .FromAttr(t => t.ANullableFloat)
              .FromAttr(t => t.ANullableDecimal);

            Assert.That(target.AString, Is.EqualTo("foo"));
            Assert.That(target.AnInt, Is.EqualTo(42));
            Assert.That(target.ABoolean, Is.True);
            Assert.That(target.ADate, Is.EqualTo(new DateTime(1970, 5, 21, 13, 55, 21, 934, DateTimeKind.Utc)));
            Assert.That(target.ADouble, Is.EqualTo(12.345D));
            Assert.That(target.AFloat, Is.EqualTo(23.456F));
            Assert.That(target.ADecimal, Is.EqualTo(34.567M));
            Assert.That(target.ANullableInt, Is.EqualTo(42));
            Assert.That(target.ANullableBoolean, Is.True);
            Assert.That(target.ANullableDate, Is.EqualTo(new DateTime(1970, 5, 21, 13, 55, 21, 934, DateTimeKind.Utc)));
            Assert.That(target.ANullableDouble, Is.EqualTo(12.345D));
            Assert.That(target.ANullableFloat, Is.EqualTo(23.456F));
            Assert.That(target.ANullableDecimal, Is.EqualTo(34.567M));
        }

        [Test]
        public void NullSerializes() {
            var target = new Target();
            var el = new XElement("data");
            el.With(target)
              .ToAttr(t => t.AString)
              .ToAttr(t => t.ANullableInt)
              .ToAttr(t => t.ANullableBoolean)
              .ToAttr(t => t.ANullableDate)
              .ToAttr(t => t.ANullableDouble)
              .ToAttr(t => t.ANullableFloat)
              .ToAttr(t => t.ANullableDecimal);

            Assert.That(el.Attr("AString"), Is.Null);
            Assert.That(el.Attr("ANullableInt"), Is.EqualTo("null"));
            Assert.That(el.Attr("ANullableBoolean"), Is.EqualTo("null"));
            Assert.That(el.Attr("ANullableDate"), Is.EqualTo("null"));
            Assert.That(el.Attr("ANullableDouble"), Is.EqualTo("null"));
            Assert.That(el.Attr("ANullableFloat"), Is.EqualTo("null"));
            Assert.That(el.Attr("ANullableDecimal"), Is.EqualTo("null"));
        }

        [Test]
        public void DeSerializeNull() {
            var target = new Target();
            var el =
                XElement.Parse(
                    "<data AString=\"null\" ANullableInt=\"null\" ANullableBoolean=\"null\" " +
                    "ANullableDate=\"null\" ANullableDouble=\"null\" " +
                    "ANullableFloat=\"null\" ANullableDecimal=\"null\"/>");
            el.With(target)
              .FromAttr(t => t.AString)
              .FromAttr(t => t.ANullableInt)
              .FromAttr(t => t.ANullableBoolean)
              .FromAttr(t => t.ANullableDate)
              .FromAttr(t => t.ANullableDouble)
              .FromAttr(t => t.ANullableFloat)
              .FromAttr(t => t.ANullableDecimal);

            Assert.That(target.AString, Is.EqualTo("null"));
            Assert.That(target.ANullableInt, Is.Null);
            Assert.That(target.ANullableBoolean, Is.Null);
            Assert.That(target.ANullableDate, Is.Null);
            Assert.That(target.ANullableDouble, Is.Null);
            Assert.That(target.ANullableFloat, Is.Null);
            Assert.That(target.ANullableDecimal, Is.Null);
        }

        private class Target {
            public string AString { get; set; }
            public int AnInt { get; set; }
            public bool ABoolean { get; set; }
            public DateTime ADate { get; set; }
            public double ADouble { get; set; }
            public float AFloat { get; set; }
            public decimal ADecimal { get; set; }
            public int? ANullableInt { get; set; }
            public bool? ANullableBoolean { get; set; }
            public DateTime? ANullableDate { get; set; }
            public double? ANullableDouble { get; set; }
            public float? ANullableFloat { get; set; }
            public decimal? ANullableDecimal { get; set; }
        }
    }
}
