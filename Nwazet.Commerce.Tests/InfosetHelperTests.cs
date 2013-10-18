using NUnit.Framework;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Nwazet.Commerce.Helpers;
using Orchard.ContentManagement.FieldStorage.InfosetStorage;
using Orchard.ContentManagement.Records;

namespace Nwazet.Commerce.Tests {
    public class InfosetHelperTests {
        [Test]
        public void StoreByNameSavesIntoInfoset() {
            var part = new TestPart();
            Helpers.PreparePart(part, "Test");
            part.Foo = 42;
            var infosetXml = part.As<InfosetPart>().Infoset.Element;
            var testPartElement = infosetXml.Element(typeof (TestPart).Name);
            Assert.That(testPartElement, Is.Not.Null);
            var fooAttribute = testPartElement.Attr<int>("Foo");

            Assert.That(part.Foo, Is.EqualTo(42));
            Assert.That(fooAttribute, Is.EqualTo(42));
        }

        [Test]
        public void RetrieveSavesIntoInfoset() {
            var part = new TestPartWithRecord();
            Helpers.PreparePart<TestPartWithRecord, TestPartWithRecordRecord>(part, "Test");
            part.Record.Foo = 42;
            var infosetXml = part.As<InfosetPart>().Infoset.Element;
            var testPartElement = infosetXml.Element(typeof (TestPartWithRecord).Name);
            Assert.That(testPartElement, Is.Null);

            var foo = part.Foo;
            testPartElement = infosetXml.Element(typeof(TestPartWithRecord).Name);
            Assert.That(testPartElement, Is.Not.Null);
            var fooAttribute = testPartElement.Attr<int>("Foo");

            Assert.That(foo, Is.EqualTo(42));
            Assert.That(fooAttribute, Is.EqualTo(42));
        }

        public class TestPart : ContentPart {
            public int Foo {
                get { return this.Retrieve<int>("Foo"); }
                set { this.Store("Foo", value); }
            }
        }

        public class TestPartWithRecordRecord : ContentPartRecord {
            public virtual int Foo { get; set; }
        }

        public class TestPartWithRecord : InfosetContentPart<TestPartWithRecordRecord> {
            public int Foo {
                get { return Retrieve(r => r.Foo); }
                set { Store(r => r.Foo, value); }
            }
        }
    }
}
