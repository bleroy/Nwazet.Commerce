using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.AdvancedSKUManagement")]
    public class AdvancedSKUsSiteSettingPart : ContentPart {
        /// <summary>
        /// When true, upon updating a ProductPart, there is a check on the uniqueness of the SKU
        /// </summary>
        public bool RequireUniqueSKU
        {
            get { return this.Retrieve(p => p.RequireUniqueSKU); }
            set { this.Store(p => p.RequireUniqueSKU, value); }
        }
        /// <summary>
        /// When true, SKUs are generated automatically for ProductParts
        /// </summary>
        public bool GenerateSKUAutomatically
        {
            get { return this.Retrieve(p => p.GenerateSKUAutomatically); }
            set { this.Store(p => p.GenerateSKUAutomatically, value); }
        }
        /// <summary>
        /// When true, if GenerateSKUAutomatically == true, prevents the SKU from being editable in the backend.
        /// </summary>
        public bool DisableSKUEditing
        {
            get { return this.Retrieve(p => p.DisableSKUEditing); }
            set { this.Store(p => p.DisableSKUEditing, value); }
        }
        /// <summary>
        /// When true, prevents changes to a ProductPart's SKU after publishing
        /// </summary>
        public bool DisableSKUUpdate
        {
            get { return this.Retrieve(p => p.DisableSKUEditing); }
            set { this.Store(p => p.DisableSKUEditing, value); }
        }
    }
}
