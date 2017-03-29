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
        /// When true, SKUs are generated automatically for ProductParts, using a systme that is similar to AutoRoute
        /// </summary>
        public bool GenerateSKUAutomatically
        {
            get { return this.Retrieve(p => p.GenerateSKUAutomatically); }
            set { this.Store(p => p.GenerateSKUAutomatically, value); }
        }

        #region Settings for AutoSKU
        /// <summary>
        /// The pattern used in SKU generation
        /// </summary>
        public string SKUPattern
        {
            get { return this.Retrieve(p => p.SKUPattern); }
            set { this.Store(p=>p.SKUPattern, value); }
        }
        /// <summary>
        /// Allow the user to change the pattern on each item
        /// </summary>
        public bool AllowCustomPattern
        {
            get { return this.Retrieve(p => p.AllowCustomPattern); }
            set { this.Store(p => p.AllowCustomPattern, value); }
        }
        #endregion
    }
}
