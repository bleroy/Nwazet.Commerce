using Nwazet.Commerce.Models;
using Orchard;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Nwazet.Commerce.Drivers {
    public class WishListListWidgetDriver : ContentPartDriver<WishListListWidgetPart> {
        private readonly IWorkContextAccessor _wca;

        public WishListListWidgetDriver(
            IWorkContextAccessor wca) {

            _wca = wca;
        }

        protected override DriverResult Display(WishListListWidgetPart part, string displayType, dynamic shapeHelper) {

            return ContentShape("WishListListWidget", () => shapeHelper.WishListListWidget(
                
                ));
        }
    }
}
