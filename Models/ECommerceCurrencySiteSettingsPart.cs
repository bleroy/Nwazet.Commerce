using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.ContentManagement;

namespace Nwazet.Commerce.Models {
    class ECommerceCurrencySiteSettingsPart : ContentPart {
        [Required,MaxLength(3)]
        public string CurrencyCode
        {
            get { return this.Retrieve(p => p.CurrencyCode); }
            set { this.Store(p => p.CurrencyCode, value); }
        }
    }
}
