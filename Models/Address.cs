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
    }
}
