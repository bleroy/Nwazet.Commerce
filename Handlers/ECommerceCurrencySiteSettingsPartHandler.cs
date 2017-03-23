using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement.Handlers;

namespace Nwazet.Commerce.Handlers {
    public class ECommerceCurrencySiteSettingsPartHandler : ContentHandler {

        public ECommerceCurrencySiteSettingsPartHandler() {
            Filters.Add(new ActivatingFilter<ECommerceCurrencySiteSettingsPart>("Site"));
        }
    }
}
