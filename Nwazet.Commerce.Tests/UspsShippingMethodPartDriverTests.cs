using System.Xml.Linq;
using NUnit.Framework;
using Nwazet.Commerce.Drivers;
using Nwazet.Commerce.Helpers;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;

namespace Nwazet.Commerce.Tests
{
    [TestFixture]
    public class UspsShippingMethodPartDriverTests
    {
        [Test]
        public void ImportGetAllDefinedProperties() {
            var doc = XElement.Parse(@"
<data>
    <UspsShippingMethodPart
        Name=""Foo""
        Size=""L""
        WidthInInches=""10""
        LengthInInches=""11""
        HeightInInches=""12""
        MaximumWeightInOunces=""1.3""
        Priority=""14""
        International=""true""
        RegisteredMail=""true""
        Insurance=""false""
        ReturnReceipt=""true""
        CertificateOfMailing=""true""
        ElectronicConfirmation=""true""/>
</data>
");
            var driver = new UspsShippingMethodPartDriver(null) as IContentPartDriver;
            var part = new UspsShippingMethodPart();
            Helpers.PreparePart<UspsShippingMethodPart, UspsShippingMethodPartRecord>(part, "UspsShippingMethod");
            var context = new ImportContentContext(part.ContentItem, doc, new ImportContentSession(null));
            driver.Importing(context);

            Assert.That(part.Name, Is.EqualTo("Foo"));
            Assert.That(part.Size, Is.EqualTo("L"));
            Assert.That(part.WidthInInches, Is.EqualTo(10));
            Assert.That(part.LengthInInches, Is.EqualTo(11));
            Assert.That(part.HeightInInches, Is.EqualTo(12));
            Assert.That(part.MaximumWeightInOunces, Is.EqualTo(1.3));
            Assert.That(part.Priority, Is.EqualTo(14));
            Assert.That(part.International, Is.True);
            Assert.That(part.RegisteredMail, Is.True);
            Assert.That(part.Insurance, Is.False);
            Assert.That(part.ReturnReceipt, Is.True);
            Assert.That(part.CertificateOfMailing, Is.True);
            Assert.That(part.ElectronicConfirmation, Is.True);
        }

        [Test]
        public void ExportSetsAllAttributes() {
            var driver = new UspsShippingMethodPartDriver(null) as IContentPartDriver;
            var part = new UspsShippingMethodPart();
            Helpers.PreparePart<UspsShippingMethodPart, UspsShippingMethodPartRecord>(part, "UspsShippingMethod");
            part.Name = "Foo";
            part.Size = "L";
            part.WidthInInches = 10;
            part.LengthInInches = 11;
            part.HeightInInches = 12;
            part.MaximumWeightInOunces = 1.3;
            part.Priority = 14;
            part.International = true;
            part.RegisteredMail = true;
            part.Insurance = false;
            part.ReturnReceipt = true;
            part.CertificateOfMailing = true;
            part.ElectronicConfirmation = true;

            var doc = new XElement("data");
            var context = new ExportContentContext(part.ContentItem, doc);
            driver.Exporting(context);
            var el = doc.Element("UspsShippingMethodPart");

            Assert.That(el, Is.Not.Null);
            Assert.That(el.Attr("Name"), Is.EqualTo("Foo"));
            Assert.That(el.Attr("Size"), Is.EqualTo("L"));
            Assert.That(el.Attr("WidthInInches"), Is.EqualTo("10"));
            Assert.That(el.Attr("LengthInInches"), Is.EqualTo("11"));
            Assert.That(el.Attr("HeightInInches"), Is.EqualTo("12"));
            Assert.That(el.Attr("MaximumWeightInOunces"), Is.EqualTo("1.3"));
            Assert.That(el.Attr("Priority"), Is.EqualTo("14"));
            Assert.That(el.Attr("International"), Is.EqualTo("true"));
            Assert.That(el.Attr("RegisteredMail"), Is.EqualTo("true"));
            Assert.That(el.Attr("Insurance"), Is.EqualTo("false"));
            Assert.That(el.Attr("ReturnReceipt"), Is.EqualTo("true"));
            Assert.That(el.Attr("CertificateOfMailing"), Is.EqualTo("true"));
            Assert.That(el.Attr("ElectronicConfirmation"), Is.EqualTo("true"));
        }
    }
}
