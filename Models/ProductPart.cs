using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using System.Collections.Generic;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Commerce")]
    public class ProductPart : ContentPart<ProductPartVersionRecord>, IProduct {
        [Required]
        public string Sku
        {
            get { return Retrieve(r => r.Sku); }
            set { Store(r => r.Sku, value); }
        }

        [Required]
        public decimal Price {
            get { return Retrieve(r => r.Price); }
            set { Store(r => r.Price, value); }
        }

        public decimal DiscountPrice {
            get {return Retrieve(r => r.DiscountPrice, -1);}
            set { Store(r => r.DiscountPrice, value); }
        }

        public bool IsDigital
        {
            get { return Retrieve(r => r.IsDigital); }
            set { Store(r => r.IsDigital, value); }
        }

        public bool ConsiderInventory
        {
            get { return Retrieve(r => r.ConsiderInventory); }
            set { Store(r => r.ConsiderInventory, value); }
        }

        public decimal? ShippingCost
        {
            get { return Retrieve(r => r.ShippingCost); }
            set { Store(r => r.ShippingCost, value); }
        }

        public double Weight
        {
            get { return Retrieve(r => r.Weight); }
            set { Store(r => r.Weight, value); }
        }

        public string Size
        {
            get { return Retrieve(r => r.Size); }
            set { Store(r => r.Size, value); }
        }

        public int Inventory
        {
            get { return Retrieve(r => r.Inventory); }
            set { Store(r => r.Inventory, value); }
        }

        public string OutOfStockMessage
        {
            get { return Retrieve(r => r.OutOfStockMessage); }
            set { Store(r => r.OutOfStockMessage, value); }
        }

        public bool AllowBackOrder
        {
            get { return Retrieve(r => r.AllowBackOrder); }
            set { Store(r => r.AllowBackOrder, value); }
        }
 
        public bool OverrideTieredPricing
        {
            get { return Retrieve(r => r.OverrideTieredPricing); }
            set { Store(r => r.OverrideTieredPricing, value); }
        }

        public IEnumerable<PriceTier> PriceTiers
        {
            get
            {
                var rawTiers = Retrieve<string>("PriceTiers");
                return PriceTier.DeserializePriceTiers(rawTiers);
            }
            set
            {
                var serializedTiers = PriceTier.SerializePriceTiers(value);
                Store("PriceTiers", serializedTiers ?? "");
            }
        }

        public int MinimumOrderQuantity
        {
            get
            {
                var minimumOrderQuantity = Retrieve(r => r.MinimumOrderQuantity);
                return minimumOrderQuantity > 1 ? minimumOrderQuantity : 1;
            }
            set
            {
                var minimumOrderQuantity = value > 1 ? value : 1;
                Store(r => r.MinimumOrderQuantity, minimumOrderQuantity); 
            }
        }

        public bool AuthenticationRequired
        {
            get { return Retrieve(r => r.AuthenticationRequired); }
            set { Store(r => r.AuthenticationRequired, value); }
        }
    }
}
