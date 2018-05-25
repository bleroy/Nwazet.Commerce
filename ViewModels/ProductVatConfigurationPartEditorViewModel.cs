using Nwazet.Commerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nwazet.Commerce.ViewModels {
    public class ProductVatConfigurationPartEditorViewModel {

        public ProductVatConfigurationPartEditorViewModel() {
            AllVatConfigurations = new List<VatConfigurationPart>();
        }

        /// <summary>
        /// This ctor allows setting the value to be displayed for the selection of the default
        /// vat category.
        /// </summary>
        /// <param name="defaultText"></param>
        public ProductVatConfigurationPartEditorViewModel(string defaultText) :
            this() {
            DefaultText = defaultText;
        }

        private string DefaultText = "Default";

        public int VatConfigurationId { get; set; }
        
        public IList<VatConfigurationPart> AllVatConfigurations { get; set; }

        public IEnumerable<SelectListItem> ListConfigurations() {
            var result = new List<SelectListItem>() {
                new SelectListItem {
                    Selected = VatConfigurationId == 0,
                    Text = DefaultText,
                    Value = "0"
                }
            };
            result.AddRange(AllVatConfigurations
                .Select(vcp => new SelectListItem {
                    Selected = vcp.Record.Id == VatConfigurationId,
                    Text = vcp.TaxProductCategory,
                    Value = vcp.Record.Id.ToString()
                }));
            return result;
        }
    }
}
