using Nwazet.Commerce.Models;
using Nwazet.Commerce.ViewModels;
using Orchard;
using Orchard.ContentManagement.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nwazet.Commerce.Services {
    public interface IOrderAdditionalInformationProvider : IDependency {

        /// <summary>
        /// Based on the OrderPart, evaluate and store the additional information
        /// to be attached to it.
        /// </summary>
        /// <param name="orderPart"></param>
        /// <remarks>This method is called when an OrderPart is created.</remarks>
        void StoreAdditionalInformation(OrderPart orderPart);

        #region Methods called by the OrderPartDriver.Editor GET method

        /// <summary>
        /// This method returns shapes to be shown along the order's metadata.
        /// </summary>
        /// <param name="orderPart"></param>
        /// <returns></returns>
        IEnumerable<dynamic> GetAdditionalOrderMetadataShapes(OrderPart orderPart);

        /// <summary>
        /// This method returns shapes to be shown along the order's status.
        /// </summary>
        /// <param name="orderPart"></param>
        /// <returns></returns>
        IEnumerable<dynamic> GetAdditionalOrderStatusShapes(OrderPart orderPart);

        /// <summary>
        /// This method returns shapes to be shown along the order's addresses.
        /// </summary>
        /// <param name="orderPart"></param>
        /// <returns></returns>
        IEnumerable<dynamic> GetAdditionalOrderAddressesShapes(OrderPart orderPart);

        /// <summary>
        /// This method returns an object describing columns to be added to the table
        /// of the order items.
        /// </summary>
        /// <param name="orderPart"></param>
        /// <returns></returns>
        IEnumerable<OrderEditorAdditionalProductInfoViewModel> GetAdditionalOrderProductsInformation(OrderPart orderPart);

        /// <summary>
        /// This method returns shapes to be shown along the order's product information.
        /// </summary>
        /// <param name="orderPart"></param>
        /// <returns></returns>
        IEnumerable<dynamic> GetAdditionalOrderProductsShapes(OrderPart orderPart);

        /// <summary>
        /// This method returns shapes to be shown along the order's tracking information.
        /// </summary>
        /// <param name="orderPart"></param>
        /// <returns></returns>
        IEnumerable<dynamic> GetAdditionalOrderTrackingShapes(OrderPart orderPart);


        #endregion

        #region Import/Export

        /// <summary>
        /// Called at the end of the Importing method for the OrderPart
        /// </summary>
        /// <param name="part"></param>
        /// <param name="context"></param>
        void Importing(OrderPart part, ImportContentContext context);

        /// <summary>
        /// Called at the end of the Exporting method for the OrderPart
        /// </summary>
        /// <param name="part"></param>
        /// <param name="context"></param>
        void Exporting(OrderPart part, ExportContentContext context);

        #endregion
    }
}
