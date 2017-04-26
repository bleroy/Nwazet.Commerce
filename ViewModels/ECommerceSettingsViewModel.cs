using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Services;

namespace Nwazet.Commerce.ViewModels {
    public class ECommerceSettingsViewModel {
        public ICurrencyProvider CurrencyProvider { get; set; }
        public dynamic Model { get; set; }
    }
}
