using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.ContentManagement;

namespace Nwazet.Commerce.Models {
    public class SelectedCurrencyProviderSiteSettingsPart : ContentPart {
        public string ActiveProvider
        {
            get { return this.Retrieve(p => p.ActiveProvider); }
            set { this.Store(p => p.ActiveProvider, value); }
        }
    }
}
