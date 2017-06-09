using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Orchard.ContentManagement;
using Orchard.MediaLibrary.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nwazet.Commerce.ViewModels {
    public class WishListElementViewModel {
        public string Title { get; set; }
        public IDictionary<int, ProductAttributeValueExtended> ProductAttributes { get; set; }
        public ContentItem ContentItem { get; set; }
        public MediaLibraryPickerField ProductImage { get; set; }
        public bool IsDigital { get; set; }
        public bool ConsiderInventory { get; set; }
        public decimal Price { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        public int Inventory { get; set; }
        public bool AllowBackOrder { get; set; }
        public string OutOfStockMessage { get; set; }
        public ICurrencyProvider CurrencyProvider { get; set; }
        public List<dynamic> ExtensionShapes { get; set; }
    }
}
