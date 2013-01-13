using System.Collections.Generic;

namespace Nwazet.Commerce.ViewModels
{
    public class UpdateShoppingCartItemViewModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public bool IsRemoved { get; set; }
        public IDictionary<int, string> AttributeIdsToValues { get; set; }
    }
}