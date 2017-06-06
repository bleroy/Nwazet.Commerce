using Orchard.ContentManagement.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nwazet.Commerce.Models {
    public class WishListListPartRecord : ContentPartRecord {
        public virtual string SerializedIds { get; set; }
        public virtual bool IsDefault { get; set; }
    }
}
