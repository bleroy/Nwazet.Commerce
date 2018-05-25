using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.ViewModels;
using Orchard.ContentManagement.Handlers;

namespace Nwazet.Commerce.Services {
    /// <summary>
    /// Base abstract implementation with "empty" methods. Inheriting form this abstract class
    /// rather than directly form the interface means you don't have to implement every single
    /// interface method, but rather just override those that are actually needed.
    /// </summary>
    public abstract class BaseOrderAdditionalInformationProvider : IOrderAdditionalInformationProvider {
        public virtual void Exporting(OrderPart part, ExportContentContext context) { }

        public virtual IEnumerable<dynamic> GetAdditionalOrderAddressesShapes(OrderPart orderPart) {
            return Enumerable.Empty<dynamic>();
        }

        public virtual IEnumerable<dynamic> GetAdditionalOrderMetadataShapes(OrderPart orderPart) {
            return Enumerable.Empty<dynamic>();
        }

        public virtual IEnumerable<OrderEditorAdditionalProductInfoViewModel> GetAdditionalOrderProductsInformation(OrderPart orderPart) {
            return Enumerable.Empty<OrderEditorAdditionalProductInfoViewModel>();
        }

        public virtual IEnumerable<dynamic> GetAdditionalOrderProductsShapes(OrderPart orderPart) {
            return Enumerable.Empty<dynamic>();
        }

        public virtual IEnumerable<dynamic> GetAdditionalOrderStatusShapes(OrderPart orderPart) {
            return Enumerable.Empty<dynamic>();
        }

        public virtual IEnumerable<dynamic> GetAdditionalOrderTrackingShapes(OrderPart orderPart) {
            return Enumerable.Empty<dynamic>();
        }

        public virtual void Importing(OrderPart part, ImportContentContext context) { }

        public virtual void StoreAdditionalInformation(OrderPart orderPart) { }
    }
}
