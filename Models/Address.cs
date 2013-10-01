using System;
using System.Xml.Linq;
using Nwazet.Commerce.Helpers;

namespace Nwazet.Commerce.Models {
    public class Address {
        /// <summary>
        /// Honorific or title
        /// </summary>
        public string Honorific { get; set; }

        /// <summary>
        /// First and second name if relevant
        /// </summary>
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string Company { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }

        /// <summary>
        /// Province, prefecture, state, or state / republic and region
        /// </summary>
        public string Province { get; set; }

        public string PostalCode { get; set; }
        public string Country { get; set; }

        public static Address Get(XElement addressElement) {
            var address = new Address();
            if (addressElement == null) return address;
            addressElement.With(address)
                .FromAttr(a => a.Honorific)
                .FromAttr(a => a.FirstName)
                .FromAttr(a => a.LastName)
                .FromAttr(a => a.Address1)
                .FromAttr(a => a.Address2)
                .FromAttr(a => a.City)
                .FromAttr(a => a.Company)
                .FromAttr(a => a.Country)
                .FromAttr(a => a.PostalCode)
                .FromAttr(a => a.Province);
            return address;
        }

        public static XElement Set(XElement addressElement, Address address) {
            return addressElement.With(address)
                .ToAttr(a => a.Honorific)
                .ToAttr(a => a.FirstName)
                .ToAttr(a => a.LastName)
                .ToAttr(a => a.Address1)
                .ToAttr(a => a.Address2)
                .ToAttr(a => a.City)
                .ToAttr(a => a.Company)
                .ToAttr(a => a.Country)
                .ToAttr(a => a.PostalCode)
                .ToAttr(a => a.Province)
                .Element;
        }

        public override string ToString() {
            return String.Join(" ", new[] {Honorific, FirstName, LastName, Company, Address1, Address2, PostalCode, City, Province, Country});
        }
    }
}
