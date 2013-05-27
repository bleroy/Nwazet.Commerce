using System;
using System.Web;
using System.Xml.Linq;
using Nwazet.Commerce.Services;

namespace Nwazet.Commerce.Tests.Stubs
{
    public class FakeUspsWebService : IUspsWebService {
        private Uri _latestUri;

        public XElement GetLatestQueryDocument() {
            var queryString = HttpUtility.ParseQueryString(_latestUri.Query);
            var xml = queryString["XML"];
            return XElement.Parse(xml);
        }

        public XElement QueryWebService(Uri url) {
            _latestUri = url;
            var queryString = HttpUtility.ParseQueryString(url.Query);
            switch (queryString["API"]) {
                case "RateV4":
                    return XElement.Parse(@"<?xml version=""1.0"" ?>
<RateV4Response>
  <Package ID=""1ST"">
    <ZipOrigination>44106</ZipOrigination>
    <ZipDestination>20770</ZipDestination>
    <Pounds>0</Pounds>
    <Ounces>3.5</Ounces>
    <FirstClassMailType>LETTER</FirstClassMailType>
    <Size>REGULAR</Size>
    <Machinable>TRUE</Machinable>
    <Zone>3</Zone>
    <Postage CLASSID=""0"">
      <MailService>First-Class Mail</MailService>
      <Rate>1.06</Rate>
      <SpecialServices>
        <SpecialService>
          <ServiceID>9</ServiceID>
          <ServiceName>Certificate of Mailing</ServiceName>
          <Available>true</Available>
          <AvailableOnline>false</AvailableOnline>
          <Price>1.20</Price>
          <PriceOnline>0</PriceOnline>
        </SpecialService>
        <SpecialService>
          <ServiceID>1</ServiceID>
          <ServiceName>Insurance</ServiceName>
          <Available>true</Available>
          <AvailableOnline>false</AvailableOnline>
          <Price>1.95</Price>
          <PriceOnline>0</PriceOnline>
          <DeclaredValueRequired>true</DeclaredValueRequired>
          <DueSenderRequired>false</DueSenderRequired>
        </SpecialService>
        <SpecialService>
          <ServiceID>5</ServiceID>
          <ServiceName>Registered Mail&lt;sup&gt;&amp;trade;&lt;/sup&gt;</ServiceName>
          <Available>true</Available>
          <AvailableOnline>false</AvailableOnline>
          <Price>11.20</Price>
          <PriceOnline>0</PriceOnline>
          <DeclaredValueRequired>true</DeclaredValueRequired>
          <DueSenderRequired>false</DueSenderRequired>
        </SpecialService>
        <SpecialService>
          <ServiceID>4</ServiceID>
          <ServiceName>Registered without Insurance</ServiceName>
          <Available>true</Available>
          <AvailableOnline>false</AvailableOnline>
          <Price>11.20</Price>
          <PriceOnline>0</PriceOnline>
        </SpecialService>
        <SpecialService>
          <ServiceID>0</ServiceID>
          <ServiceName>Certified Mail&lt;sup&gt;&amp;reg;&lt;/sup&gt;</ServiceName>
          <Available>true</Available>
          <AvailableOnline>false</AvailableOnline>
          <Price>3.10</Price>
          <PriceOnline>0</PriceOnline>
        </SpecialService>
        <SpecialService>
          <ServiceID>6</ServiceID>
          <ServiceName>Collect on Delivery</ServiceName>
          <Available>true</Available>
          <AvailableOnline>false</AvailableOnline>
          <Price>6.45</Price>
          <PriceOnline>0</PriceOnline>
          <DeclaredValueRequired>true</DeclaredValueRequired>
          <DueSenderRequired>true</DueSenderRequired>
        </SpecialService>
      </SpecialServices>
    </Postage>
    <Postage CLASSID=""1"">
      <MailService>Priority Mail&lt;sup&gt;&amp;reg;&lt;/sup&gt;</MailService>
      <Rate>24.85</Rate>
      <SpecialServices>
        <SpecialService>
          <ServiceID>1</ServiceID>
          <ServiceName>Insurance</ServiceName>
          <Available>true</Available>
          <AvailableOnline>true</AvailableOnline>
          <Price>13.15</Price>
          <PriceOnline>13.15</PriceOnline>
          <DeclaredValueRequired>true</DeclaredValueRequired>
          <DueSenderRequired>false</DueSenderRequired>
        </SpecialService>
        <SpecialService>
          <ServiceID>13</ServiceID>
          <ServiceName>Delivery Confirmation&lt;sup&gt;&amp;trade;&lt;/sup&gt;</ServiceName>
          <Available>true</Available>
          <AvailableOnline>true</AvailableOnline>
          <Price>0.00</Price>
          <PriceOnline>0.00</PriceOnline>
        </SpecialService>
        <SpecialService>
          <ServiceID>4</ServiceID>
          <ServiceName>Restricted Delivery</ServiceName>
          <Available>true</Available>
          <AvailableOnline>false</AvailableOnline>
          <Price>4.75</Price>
          <PriceOnline>0</PriceOnline>
        </SpecialService>
        <SpecialService>
          <ServiceID>8</ServiceID>
          <ServiceName>Return Receipt</ServiceName>
          <Available>true</Available>
          <AvailableOnline>false</AvailableOnline>
          <Price>2.55</Price>
          <PriceOnline>0</PriceOnline>
        </SpecialService>
        <SpecialService>
          <ServiceID>16</ServiceID>
          <ServiceName>Return Receipt Electronic</ServiceName>
          <Available>true</Available>
          <AvailableOnline>false</AvailableOnline>
          <Price>1.25</Price>
          <PriceOnline>0</PriceOnline>
        </SpecialService>
        <SpecialService>
          <ServiceID>15</ServiceID>
          <ServiceName>Signature Confirmation&lt;sup&gt;&amp;trade;&lt;/sup&gt;</ServiceName>
          <Available>true</Available>
          <AvailableOnline>true</AvailableOnline>
          <Price>2.70</Price>
          <PriceOnline>2.20</PriceOnline>
        </SpecialService>
        <SpecialService>
          <ServiceID>19</ServiceID>
          <ServiceName>Adult Signature Required</ServiceName>
          <Available>false</Available>
          <AvailableOnline>true</AvailableOnline>
          <Price>0</Price>
          <PriceOnline>4.95</PriceOnline>
        </SpecialService>
        <SpecialService>
          <ServiceID>20</ServiceID>
          <ServiceName> Adult Signature Restricted Delivery</ServiceName>
          <Available>false</Available>
          <AvailableOnline>true</AvailableOnline>
          <Price>0</Price>
          <PriceOnline>5.15</PriceOnline>
        </SpecialService>
      </SpecialServices>
    </Postage>
    <Postage CLASSID=""1"">
      <MailService>Priority Mail&lt;sup&gt;&amp;reg;&lt;/sup&gt;</MailService>
      <Rate>18.35</Rate>
    </Postage>
    <Postage CLASSID=""22"">
      <MailService>Priority Mail&lt;sup&gt;&amp;reg;&lt;/sup&gt; Large Flat Rate Box</MailService>
      <Rate>14.85</Rate>
    </Postage>
    <Postage CLASSID=""17"">
      <MailService>Priority Mail&lt;sup&gt;&amp;reg;&lt;/sup&gt; Medium Flat Rate Box</MailService>
      <Rate>12.35</Rate>
    </Postage>
    <Postage CLASSID=""28"">
      <MailService>Priority Mail&lt;sup&gt;&amp;reg;&lt;/sup&gt; Small Flat Rate Box</MailService>
      <Rate>5.80</Rate>
    </Postage>
    <Postage CLASSID=""16"">
      <MailService>Priority Mail&lt;sup&gt;&amp;reg;&lt;/sup&gt; Flat Rate Envelope</MailService>
      <Rate>5.60</Rate>
    </Postage>
    <Postage CLASSID=""44"">
      <MailService>Priority Mail&lt;sup&gt;&amp;reg;&lt;/sup&gt; Legal Flat Rate Envelope</MailService>
      <Rate>5.75</Rate>
    </Postage>
    <Postage CLASSID=""29"">
      <MailService>Priority Mail&lt;sup&gt;&amp;reg;&lt;/sup&gt; Padded Flat Rate Envelope</MailService>
      <Rate>5.95</Rate>
    </Postage>
    <Postage CLASSID=""38"">
      <MailService>Priority Mail&lt;sup&gt;&amp;reg;&lt;/sup&gt; Gift Card Flat Rate Envelope</MailService>
      <Rate>5.60</Rate>
    </Postage>
    <Postage CLASSID=""42"">
      <MailService>Priority Mail&lt;sup&gt;&amp;reg;&lt;/sup&gt; Small Flat Rate Envelope</MailService>
      <Rate>5.60</Rate>
    </Postage>
    <Postage CLASSID=""40"">
      <MailService>Priority Mail&lt;sup&gt;&amp;reg;&lt;/sup&gt; Window Flat Rate Envelope</MailService>
      <Rate>5.60</Rate>
    </Postage>
    <Postage CLASSID=""4"">
      <MailService>Standard Post&lt;sup&gt;&amp;reg;&lt;/sup&gt;</MailService>
      <Rate>18.35</Rate>
    </Postage>
    <Postage CLASSID=""6"">
      <MailService>Media Mail&lt;sup&gt;&amp;reg;&lt;/sup&gt;</MailService>
      <Rate>6.52</Rate>
    </Postage>
    <Postage CLASSID=""7"">
      <MailService>Library Mail</MailService>
      <Rate>6.21</Rate>
    </Postage>
    <Restrictions>
      A1. Mail addressed to “Any Servicemember” or similar <!--1772 skipped-->.
    </Restrictions>
  </Package>
</RateV4Response>");
                case "IntlRateV2":
                    return XElement.Parse(@"<?xml version=""1.0"" ?>
<IntlRateV2Response>
  <Package ID=""1ST"">
    <Prohibitions>
      An issue of a publication <!--2143 suppressed-->.
    </Prohibitions>
    <Restrictions>
      Coins; banknotes; curren<!--1558 suppressed-->
    </Restrictions>
    <Observations>
      1. Banknotes valued at <!--3059 suppressed-->.
    </Observations>
    <CustomsForms>
      First-Class Mail Intern <!--358 suppressed-->)
    </CustomsForms>
    <ExpressMail>
      Country Code: CA Recipro<!--2036 suppressed-->
    </ExpressMail>
    <AreasServed>Please reference Express Mail for Areas Served.</AreasServed>
    <AdditionalRestrictions>No Additional Restrictions Data found.</AdditionalRestrictions>
    <Service ID=""4"">
      <Pounds>15</Pounds>
      <Ounces>0</Ounces>
      <Machinable>True</Machinable>
      <MailType>Package</MailType>
      <GXG>
        <POBoxFlag>Y</POBoxFlag>
        <GiftFlag>Y</GiftFlag>
      </GXG>
      <Container>RECTANGULAR</Container>
      <Size>LARGE</Size>
      <Width>10</Width>
      <Length>15</Length>
      <Height>10</Height>
      <Girth>0</Girth>
      <Country>CANADA</Country>
      <Postage>117.05</Postage>
      <ExtraServices>
        <ExtraService>
          <ServiceID>0</ServiceID>
          <ServiceName>Registered Mail</ServiceName>
          <Available>True</Available>
          <Price>1.50</Price>
          <DeclaredValueRequired>True</DeclaredValueRequired>
        </ExtraService>
        <ExtraService>
          <ServiceID>1</ServiceID>
          <ServiceName>Insurance</ServiceName>
          <Available>True</Available>
          <Price>1.00</Price>
          <DeclaredValueRequired>True</DeclaredValueRequired>
        </ExtraService>
        <ExtraService>
          <ServiceID>2</ServiceID>
          <ServiceName>Return Receipt</ServiceName>
          <Available>True</Available>
          <Price>1.70</Price>
          <DeclaredValueRequired>True</DeclaredValueRequired>
        </ExtraService>
        <ExtraService>
          <ServiceID>6</ServiceID>
          <ServiceName>Certificate Of Mailing</ServiceName>
          <Available>True</Available>
          <Price>1.18</Price>
          <DeclaredValueRequired>True</DeclaredValueRequired>
        </ExtraService>
        <ExtraService>
          <ServiceID>9</ServiceID>
          <ServiceName>Electronic Confirmation</ServiceName>
          <Available>True</Available>
          <Price>1.04</Price>
          <DeclaredValueRequired>True</DeclaredValueRequired>
        </ExtraService>
      </ExtraServices>
      <ValueOfContents>200.00</ValueOfContents>
      <SvcCommitments>1 - 3 business days</SvcCommitments>
      <SvcDescription>Global Express Guaranteed&lt;sup&gt;&amp;reg;&lt;/sup&gt; (GXG)**</SvcDescription>
      <MaxDimensions>Max. length 46"", width 35"", height 46"" and max. length plus girth combined 108""</MaxDimensions>
      <MaxWeight>70</MaxWeight>
      <GXGLocations>
        <PostOffice>
          <Name>WILKES BARRE PD&amp;C</Name>
          <Address>300 S MAIN ST</Address>
          <City>WILKES BARRE</City>
          <State>PA</State>
          <ZipCode />
          <RetailGXGCutOffTime>5:00 PM</RetailGXGCutOffTime>
          <SaturDayCutOffTime>2:00 PM</SaturDayCutOffTime>
        </PostOffice>
      </GXGLocations>
    </Service>
    <Service ID=""12"">
      <Pounds>15</Pounds>
      <Ounces>0</Ounces>
      <Machinable>True</Machinable>
      <MailType>Package</MailType>
      <GXG>
        <POBoxFlag>Y</POBoxFlag>
        <GiftFlag>Y</GiftFlag>
      </GXG>
      <Container>RECTANGULAR</Container>
      <Size>LARGE</Size>
      <Width>10</Width>
      <Length>15</Length>
      <Height>10</Height>
      <Girth>0</Girth>
      <Country>CANADA</Country>
      <Postage>117.05</Postage>
      <ExtraServices>
        <ExtraService>
          <ServiceID>1</ServiceID>
          <ServiceName>Insurance</ServiceName>
          <Available>True</Available>
          <Price>1.00</Price>
          <DeclaredValueRequired>True</DeclaredValueRequired>
        </ExtraService>
      </ExtraServices>
      <ValueOfContents>200.00</ValueOfContents>
      <SvcCommitments>1 - 3 business days</SvcCommitments>
      <SvcDescription>USPS GXG&lt;sup&gt;&amp;trade;&lt;/sup&gt; Envelopes**</SvcDescription>
      <MaxDimensions>
        USPS-Produced regular size cardboard envelope (12-1/2"" x 9-1/2""), the legal-sized cardboard envelope (15"" x
        9-1/2"") and the GXG Tyvek envelope (15-1/2"" x 12-1/2"")
      </MaxDimensions>
      <MaxWeight>70</MaxWeight>
      <GXGLocations>
        <PostOffice>
          <Name>WILKES BARRE PD&amp;C</Name>
          <Address>300 S MAIN ST</Address>
          <City>WILKES BARRE</City>
          <State>PA</State>
          <ZipCode />
          <RetailGXGCutOffTime>5:00 PM</RetailGXGCutOffTime>
          <SaturDayCutOffTime>2:00 PM</SaturDayCutOffTime>
        </PostOffice>
      </GXGLocations>
    </Service>
    <Service ID=""1"">
      <Pounds>15</Pounds>
      <Ounces>0</Ounces>
      <Machinable>True</Machinable>
      <MailType>Package</MailType>
      <Container>RECTANGULAR</Container>
      <Size>LARGE</Size>
      <Width>10</Width>
      <Length>15</Length>
      <Height>10</Height>
      <Girth>0</Girth>
      <Country>CANADA</Country>
      <Postage>96.30</Postage>
      <ExtraServices>
        <ExtraService>
          <ServiceID>1</ServiceID>
          <ServiceName>Insurance</ServiceName>
          <Available>True</Available>
          <Price>0.80</Price>
          <DeclaredValueRequired>True</DeclaredValueRequired>
        </ExtraService>
      </ExtraServices>
      <ValueOfContents>200.00</ValueOfContents>
      <SvcCommitments>3 - 5 business days</SvcCommitments>
      <SvcDescription>Express Mail&lt;sup&gt;&amp;reg;&lt;/sup&gt; International</SvcDescription>
      <MaxDimensions>Max. length 42"", max. length plus girth combined 79""</MaxDimensions>
      <MaxWeight>66</MaxWeight>
    </Service>
    <Service ID=""2"">
      <Pounds>15</Pounds>
      <Ounces>0</Ounces>
      <Machinable>True</Machinable>
      <MailType>Package</MailType>
      <Container>RECTANGULAR</Container>
      <Size>LARGE</Size>
      <Width>10</Width>
      <Length>15</Length>
      <Height>10</Height>
      <Girth>0</Girth>
      <Country>CANADA</Country>
      <Postage>62.15</Postage>
      <ExtraServices>
        <ExtraService>
          <ServiceID>1</ServiceID>
          <ServiceName>Insurance</ServiceName>
          <Available>True</Available>
          <Price>2.30</Price>
          <DeclaredValueRequired>True</DeclaredValueRequired>
        </ExtraService>
        <ExtraService>
          <ServiceID>6</ServiceID>
          <ServiceName>Certificate of Mailing</ServiceName>
          <Available>True</Available>
          <Price>1.15</Price>
        </ExtraService>
      </ExtraServices>
      <ValueOfContents>200.00</ValueOfContents>
      <ParcelIndemnityCoverage>108.62</ParcelIndemnityCoverage>
      <SvcCommitments>6 - 10 business days</SvcCommitments>
      <SvcDescription>Priority Mail&lt;sup&gt;&amp;reg;&lt;/sup&gt; International</SvcDescription>
      <MaxDimensions>Max. length 79"", max. length plus girth 108""</MaxDimensions>
      <MaxWeight>66</MaxWeight>
    </Service>
    <Service ID=""12"">
      <Pounds>0</Pounds>
      <Ounces>3</Ounces>
      <MailType>Envelope</MailType>
      <Container />
      <Size>REGULAR</Size>
      <Width>0</Width>
      <Length>0</Length>
      <Height>0</Height>
      <Girth>0</Girth>
      <Country>ALGERIA</Country>
      <Postage>104.50</Postage>
      <ExtraServices>
        <ExtraService>
          <ServiceID>1</ServiceID>
          <ServiceName>Insurance</ServiceName>
          <Available>True</Available>
          <Price>1.00</Price>
          <DeclaredValueRequired>True</DeclaredValueRequired>
        </ExtraService>
      </ExtraServices>
      <ValueOfContents>750.00</ValueOfContents>
      <SvcCommitments>1 - 3 business days</SvcCommitments>
      <SvcDescription>USPS GXG&lt;sup&gt;&amp;trade;&lt;/sup&gt; Envelopes**</SvcDescription>
      <MaxDimensions>
        USPS-Produced regular size cardboard envelope (12-1/2"" x 9-1/2""), the legal-sized cardboard envelope (15"" x
        9-1/2"") and the GXG Tyvek envelope (15-1/2"" x 12-1/2"")
      </MaxDimensions>
      <MaxWeight>70</MaxWeight>
    </Service>
    <Service ID=""1"">
      <Pounds>0</Pounds>
      <Ounces>3</Ounces>
      <MailType>Envelope</MailType>
      <Container />
      <Size>REGULAR</Size>
      <Width>0</Width>
      <Length>0</Length>
      <Height>0</Height>
      <Girth>0</Girth>
      <Country>ALGERIA</Country>
      <Postage>46.00</Postage>
      <ExtraServices>
        <ExtraService>
          <ServiceID>1</ServiceID>
          <ServiceName>Insurance</ServiceName>
          <Available>True</Available>
          <Price>2.35</Price>
          <DeclaredValueRequired>True</DeclaredValueRequired>
        </ExtraService>
      </ExtraServices>
      <ValueOfContents>750.00</ValueOfContents>
      <SvcCommitments>3 - 5 business days</SvcCommitments>
      <SvcDescription>Express Mail&lt;sup&gt;&amp;reg;&lt;/sup&gt; International</SvcDescription>
      <MaxDimensions>Max. length 36"", max. length plus girth 79""</MaxDimensions>
      <MaxWeight>44</MaxWeight>
    </Service>
    <Service ID=""10"">
      <Pounds>0</Pounds>
      <Ounces>3</Ounces>
      <MailType>Envelope</MailType>
      <Container />
      <Size>REGULAR</Size>
      <Width>0</Width>
      <Length>0</Length>
      <Height>0</Height>
      <Girth>0</Girth>
      <Country>ALGERIA</Country>
      <Postage>44.95</Postage>
      <ExtraServices>
        <ExtraService>
          <ServiceID>1</ServiceID>
          <ServiceName>Insurance</ServiceName>
          <Available>True</Available>
          <Price>2.35</Price>
          <DeclaredValueRequired>True</DeclaredValueRequired>
        </ExtraService>
      </ExtraServices>
      <ValueOfContents>750.00</ValueOfContents>
      <SvcCommitments>3 - 5 business days</SvcCommitments>
      <SvcDescription>Express Mail&lt;sup&gt;&amp;reg;&lt;/sup&gt; International Flat Rate Envelope</SvcDescription>
      <MaxDimensions>USPS-Produced Envelope: 12-1/2"" x 9-1/2""</MaxDimensions>
      <MaxWeight>44</MaxWeight>
    </Service>
    <Service ID=""17"">
      <Pounds>0</Pounds>
      <Ounces>3</Ounces>
      <MailType>Envelope</MailType>
      <Container />
      <Size>REGULAR</Size>
      <Width>0</Width>
      <Length>0</Length>
      <Height>0</Height>
      <Girth>0</Girth>
      <Country>ALGERIA</Country>
      <Postage>44.95</Postage>
      <ExtraServices>
        <ExtraService>
          <ServiceID>1</ServiceID>
          <ServiceName>Insurance</ServiceName>
          <Available>True</Available>
          <Price>2.35</Price>
          <DeclaredValueRequired>True</DeclaredValueRequired>
        </ExtraService>
      </ExtraServices>
      <ValueOfContents>750.00</ValueOfContents>
      <SvcCommitments>3 - 5 business days</SvcCommitments>
      <SvcDescription>Express Mail&lt;sup&gt;&amp;reg;&lt;/sup&gt; International Legal Flat Rate Envelope</SvcDescription>
      <MaxDimensions>USPS-Produced Envelope: 15"" x 9-1/2""</MaxDimensions>
      <MaxWeight>44</MaxWeight>
    </Service>
    <Service ID=""27"">
      <Pounds>0</Pounds>
      <Ounces>3</Ounces>
      <MailType>Envelope</MailType>
      <Container />
      <Size>REGULAR</Size>
      <Width>0</Width>
      <Length>0</Length>
      <Height>0</Height>
      <Girth>0</Girth>
      <Country>ALGERIA</Country>
      <Postage>44.95</Postage>
      <ExtraServices>
        <ExtraService>
          <ServiceID>1</ServiceID>
          <ServiceName>Insurance</ServiceName>
          <Available>True</Available>
          <Price>2.35</Price>
          <DeclaredValueRequired>True</DeclaredValueRequired>
        </ExtraService>
      </ExtraServices>
      <ValueOfContents>750.00</ValueOfContents>
      <SvcCommitments>3 - 5 business days</SvcCommitments>
      <SvcDescription>Express Mail&lt;sup&gt;&amp;reg;&lt;/sup&gt; International Padded Flat Rate Envelope</SvcDescription>
      <MaxDimensions>USPS-Produced Envelope: 15"" x 9-1/2""</MaxDimensions>
      <MaxWeight>44</MaxWeight>
    </Service>
    <Service ID=""2"">
      <Pounds>0</Pounds>
      <Ounces>3</Ounces>
      <MailType>Envelope</MailType>
      <Container />
      <Size>REGULAR</Size>
      <Width>0</Width>
      <Length>0</Length>
      <Height>0</Height>
      <Girth>0</Girth>
      <Country>ALGERIA</Country>
      <Postage>36.75</Postage>
      <ExtraServices>
        <ExtraService>
          <ServiceID>6</ServiceID>
          <ServiceName>Certificate of Mailing</ServiceName>
          <Available>True</Available>
          <Price>1.20</Price>
        </ExtraService>
      </ExtraServices>
      <ValueOfContents>750.00</ValueOfContents>
      <InsComment>SERVICE</InsComment>
      <ParcelIndemnityCoverage>64.66</ParcelIndemnityCoverage>
      <SvcCommitments>6 - 10 business days</SvcCommitments>
      <SvcDescription>Priority Mail&lt;sup&gt;&amp;reg;&lt;/sup&gt; International</SvcDescription>
      <MaxDimensions>Max. length 42"", max. length plus girth combined 79""</MaxDimensions>
      <MaxWeight>44</MaxWeight>
    </Service>
    <Service ID=""8"">
      <Pounds>0</Pounds>
      <Ounces>3</Ounces>
      <MailType>Envelope</MailType>
      <Container />
      <Size>REGULAR</Size>
      <Width>0</Width>
      <Length>0</Length>
      <Height>0</Height>
      <Girth>0</Girth>
      <Country>ALGERIA</Country>
      <Postage>23.95</Postage>
      <ExtraServices>
        <ExtraService>
          <ServiceID>6</ServiceID>
          <ServiceName>Certificate of Mailing</ServiceName>
          <Available>True</Available>
          <Price>1.20</Price>
        </ExtraService>
      </ExtraServices>
      <ValueOfContents>750.00</ValueOfContents>
      <InsComment>SERVICE</InsComment>
      <SvcCommitments>6 - 10 business days</SvcCommitments>
      <SvcDescription>Priority Mail&lt;sup&gt;&amp;reg;&lt;/sup&gt; International Flat Rate Envelope**</SvcDescription>
      <MaxDimensions>
        USPS-Produced Envelope: 12-1/2"" x 9-1/2"".&lt;br/&gt;Maximum weight 4 pounds.</MaxDimensions>
      <MaxWeight>4</MaxWeight>
    </Service>
    <Service ID=""22"">
      <Pounds>0</Pounds>
      <Ounces>3</Ounces>
      <MailType>Envelope</MailType>
      <Container />
      <Size>REGULAR</Size>
      <Width>0</Width>
      <Length>0</Length>
      <Height>0</Height>
      <Girth>0</Girth>
      <Country>ALGERIA</Country>
      <Postage>23.95</Postage>
      <ExtraServices>
        <ExtraService>
          <ServiceID>6</ServiceID>
          <ServiceName>Certificate of Mailing</ServiceName>
          <Available>True</Available>
          <Price>1.20</Price>
        </ExtraService>
      </ExtraServices>
      <ValueOfContents>750.00</ValueOfContents>
      <InsComment>SERVICE</InsComment>
      <SvcCommitments>6 - 10 business days</SvcCommitments>
      <SvcDescription>Priority Mail&lt;sup&gt;&amp;reg;&lt;/sup&gt; International Legal Flat Rate Envelope**</SvcDescription>
      <MaxDimensions>
        USPS-Produced Envelope: 15-1/2"" x 9-1/2"".&lt;br/&gt;Maximum weight 4 pounds.</MaxDimensions>
      <MaxWeight>4</MaxWeight>
    </Service>
    <Service ID=""23"">
      <Pounds>0</Pounds>
      <Ounces>3</Ounces>
      <MailType>Envelope</MailType>
      <Container />
      <Size>REGULAR</Size>
      <Width>0</Width>
      <Length>0</Length>
      <Height>0</Height>
      <Girth>0</Girth>
      <Country>ALGERIA</Country>
      <Postage>23.95</Postage>
      <ExtraServices>
        <ExtraService>
          <ServiceID>6</ServiceID>
          <ServiceName>Certificate of Mailing</ServiceName>
          <Available>True</Available>
          <Price>1.20</Price>
        </ExtraService>
      </ExtraServices>
      <ValueOfContents>750.00</ValueOfContents>
      <InsComment>SERVICE</InsComment>
      <SvcCommitments>6 - 10 business days</SvcCommitments>
      <SvcDescription>Priority Mail&lt;sup&gt;&amp;reg;&lt;/sup&gt; International Padded Flat Rate Envelope**</SvcDescription>
      <MaxDimensions>
        USPS-Produced Envelope: 12-1/2"" x 9-1/2"".&lt;br/&gt;Maximum weight 4 pounds.</MaxDimensions>
      <MaxWeight>4</MaxWeight>
    </Service>
    <Service ID=""18"">
      <Pounds>0</Pounds>
      <Ounces>3</Ounces>
      <MailType>Envelope</MailType>
      <Container />
      <Size>REGULAR</Size>
      <Width>0</Width>
      <Length>0</Length>
      <Height>0</Height>
      <Girth>0</Girth>
      <Country>ALGERIA</Country>
      <Postage>23.95</Postage>
      <ExtraServices>
        <ExtraService>
          <ServiceID>6</ServiceID>
          <ServiceName>Certificate of Mailing</ServiceName>
          <Available>True</Available>
          <Price>1.20</Price>
        </ExtraService>
       </ExtraServices>
        <ValueOfContents>750.00</ValueOfContents>
        <InsComment>SERVICE</InsComment>
        <SvcCommitments>6 - 10 business days</SvcCommitments>
        <SvcDescription>Priority Mail&lt;sup&gt;&amp;reg;&lt;/sup&gt; International Gift Card Flat Rate Envelope**</SvcDescription>
        <MaxDimensions>
          USPS-Produced Envelope: 10"" x 7"".&lt;br/&gt;Maximum weight 4 pounds.</MaxDimensions>
        <MaxWeight>4</MaxWeight>
      </Service>
    <Service ID=""20"">
      <Pounds>0</Pounds>
      <Ounces>3</Ounces>
      <MailType>Envelope</MailType>
      <Container />
      <Size>REGULAR</Size>
      <Width>0</Width>
      <Length>0</Length>
      <Height>0</Height>
      <Girth>0</Girth>
      <Country>ALGERIA</Country>
      <Postage>23.95</Postage>
      <ExtraServices>
        <ExtraService>
          <ServiceID>6</ServiceID>
          <ServiceName>Certificate of Mailing</ServiceName>
          <Available>True</Available>
          <Price>1.20</Price>
        </ExtraService>
      </ExtraServices>
      <ValueOfContents>750.00</ValueOfContents>
      <InsComment>SERVICE</InsComment>
      <SvcCommitments>6 - 10 business days</SvcCommitments>
      <SvcDescription>Priority Mail&lt;sup&gt;&amp;reg;&lt;/sup&gt; International Small Flat Rate Envelope**</SvcDescription>
      <MaxDimensions>
        USPS-Produced Envelope: 10"" x 6"".&lt;br/&gt;Maximum weight 4 pounds.</MaxDimensions>
      <MaxWeight>4</MaxWeight>
    </Service>
    <Service ID=""19"">
      <Pounds>0</Pounds>
      <Ounces>3</Ounces>
      <MailType>Envelope</MailType>
      <Container />
      <Size>REGULAR</Size>
      <Width>0</Width>
      <Length>0</Length>
      <Height>0</Height>
      <Girth>0</Girth>
      <Country>ALGERIA</Country>
      <Postage>23.95</Postage>
      <ExtraServices>
        <ExtraService>
          <ServiceID>6</ServiceID>
          <ServiceName>Certificate of Mailing</ServiceName>
          <Available>True</Available>
          <Price>1.20</Price>
        </ExtraService>
      </ExtraServices>
      <ValueOfContents>750.00</ValueOfContents>
      <InsComment>SERVICE</InsComment>
      <SvcCommitments>6 - 10 business days</SvcCommitments>
      <SvcDescription>Priority Mail&lt;sup&gt;&amp;reg;&lt;/sup&gt; International Window Flat Rate Envelope**</SvcDescription>
      <MaxDimensions>
        USPS-Produced Envelope: 10"" x 5"".&lt;br/&gt;Maximum weight 4 pounds.</MaxDimensions>
      <MaxWeight>4</MaxWeight>
    </Service>
    <Service ID=""13"">
      <Pounds>0</Pounds>
      <Ounces>3</Ounces>
      <MailType>Envelope</MailType>
      <Container />
      <Size>REGULAR</Size>
      <Width>0</Width>
      <Length>0</Length>
      <Height>0</Height>
      <Girth>0</Girth>
      <Country>ALGERIA</Country>
      <Postage>2.70</Postage>
      <ExtraServices>
        <ExtraService>
          <ServiceID>6</ServiceID>
          <ServiceName>Certificate of Mailing</ServiceName>
          <Available>True</Available>
          <Price>1.20</Price>
        </ExtraService>
      </ExtraServices>
      <ValueOfContents>750.00</ValueOfContents>
      <InsComment>SERVICE</InsComment>
      <SvcCommitments>Varies by country</SvcCommitments>
      <SvcDescription>First-Class Mail&lt;sup&gt;&amp;reg;&lt;/sup&gt; International Letter**</SvcDescription>
      <MaxDimensions>Max. length 11-1/2"", height 6-1/8"" or thickness 1/4"".</MaxDimensions>
      <MaxWeight>0.21875</MaxWeight>
    </Service>
  </Package>
</IntlRateV2Response>");
                default:
                    return XElement.Parse(@"<?xml version=""1.0"" ?>
<Error> 
    <Number>0</Number> 
    <Source>Nwazet.Commerce</Source> 
    <Description>Unrecognized API</Description> 
    <HelpFile></HelpFile> 
    <HelpContext></HelpContext> 
</Error> ");
            }
        }
    }
}
