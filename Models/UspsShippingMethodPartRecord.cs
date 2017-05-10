using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Usps.Shipping")]
    public class UspsShippingMethodPartRecord : ContentPartRecord {
        public virtual string Name { get; set; }
        public virtual string Size { get; set; }
        public virtual string Container { get; set; }
        public virtual decimal Markup { get; set; }
        public virtual int WidthInInches { get; set; }
        public virtual int LengthInInches { get; set; }
        public virtual int HeightInInches { get; set; }
        public virtual double MaximumWeightInOunces { get; set; }
        public virtual double WeightPaddingInOunces { get; set; }
        public virtual int MinimumQuantity { get; set; }
        public virtual int MaximumQuantity { get; set; }
        public virtual bool CountDistinct { get; set; }
        public virtual string ServiceNameValidationExpression { get; set; }
        public virtual string ServiceNameExclusionExpression { get; set; }
        public virtual int Priority { get; set; }
        public virtual bool International { get; set; }
        public virtual bool RegisteredMail { get; set; }
        public virtual bool Insurance { get; set; }
        public virtual bool ReturnReceipt { get; set; }
        public virtual bool CertificateOfMailing { get; set; }
        public virtual bool ElectronicConfirmation { get; set; }
    }
}
