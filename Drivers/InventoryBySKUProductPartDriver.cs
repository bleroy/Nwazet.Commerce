using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;
using System.Linq;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Nwazet.InventoryBySKU")]
    public class InventoryBySKUProductPartDriver : ContentPartDriver<ProductPart> {

        private readonly IProductInventoryService _productInventoryService;
        public InventoryBySKUProductPartDriver(
            IProductInventoryService productInventoryService) {

            _productInventoryService = productInventoryService;
        }

        protected override string Prefix
        {
            get { return "NwazetCommerceProduct"; }
        }

        protected override DriverResult Editor(ProductPart part, dynamic shapeHelper) {

            var model = new InventoryBySKUProductEditorViewModel() {
                Product = part
            };
            model.SameInventoryItems = _productInventoryService.GetProductsWithSameInventory(part)
                .Where(pa => pa.ContentItem.Id != part.ContentItem.Id);

            return ContentShape("Part_Product_InventoryBySKUEdit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/InventoryBySKUProduct",
                    Model: model,
                    Prefix: Prefix));
        }

        protected override DriverResult Editor(ProductPart part, IUpdateModel updater, dynamic shapeHelper) {
            var model = new InventoryBySKUProductEditorViewModel() {
                Product = part
            };
            if (updater.TryUpdateModel(model, Prefix, null, null)) {
                //update the Inventory across all products that share it
                _productInventoryService.SynchronizeInventories(part);
            }
            return Editor(part, shapeHelper);
        }
    }
}
