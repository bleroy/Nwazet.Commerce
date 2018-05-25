using Orchard.Data.Conventions;
using Orchard.Environment.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.AdvancedVAT")]
    public class OrderVatRecord {
        public virtual int Id { get; set; } // Primary key

        public virtual OrderPartRecord OrderPartRecord { get; set; }
        [StringLengthMax]
        public virtual string Information { get; set; }
    }
}
