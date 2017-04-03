using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;

namespace Nwazet.Commerce.ViewModels {
    public class AdvancedSKUProductEditorViewModel {
        public ProductPart Product { get; set; }
        public string CurrentSku { get; set; }
        public AdvancedSKUsSiteSettingPart Settings { get; set; }
        public string SkuPattern { get; set; }
    }
}
