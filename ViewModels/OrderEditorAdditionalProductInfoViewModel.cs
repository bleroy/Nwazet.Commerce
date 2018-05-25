using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nwazet.Commerce.ViewModels {
    /// <summary>
    /// Instances of this class represent additional information being added to each product
    /// in an order.
    /// </summary>
    public class OrderEditorAdditionalProductInfoViewModel {
        /// <summary>
        /// The title used as column name for the information.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// The css class for the element that will represent the header for this
        /// information.
        /// </summary>
        public string HeaderClass { get; set; }

        /// <summary>
        /// This Dictionary contains the information we wish to add. The key is the
        /// Id of the product the information is about.
        /// </summary>
        public Dictionary<int, string> Information { get; set; }
        /// <summary>
        /// The css class for the element that will represent the information.
        /// </summary>
        public string InformationClass { get; set; }
    }
}
