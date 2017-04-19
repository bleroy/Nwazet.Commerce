using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Nwazet.Commerce.Migrations {
    [OrchardFeature("Nwazet.Orders")]
    public class OrderMigrations : DataMigrationImpl {

        private readonly IContentManager _contentManager;
        private readonly ICurrencyProvider _currencyProvider;

        public OrderMigrations(
            IContentManager contentManager,
            ICurrencyProvider currencyProvider) {

            _contentManager = contentManager;
            _currencyProvider = currencyProvider;
        }

        public int Create() {
            SchemaBuilder.CreateTable("OrderPartRecord", table => table
                .ContentPartRecord()
                .Column<string>("Status")
                .Column<string>("Contents", column => column.Unlimited())
                .Column<string>("Customer", column => column.Unlimited())
                .Column<string>("Activity", column => column.Unlimited())
                .Column<string>("TrackingUrl")
                .Column<string>("Password")
                .Column<bool>("IsTestOrder"));

            ContentDefinitionManager.AlterTypeDefinition("Order", type => type
                .DisplayedAs("Order")
                .WithPart("Order")
                .WithPart("OrderPart")
                .WithPart("CommonPart", p => p.WithSetting("OwnerEditorSettings.ShowOwnerEditor", "false"))
                .WithPart("IdentityPart"));

            return 2;
        }

        public int UpdateFrom1() {
            ContentDefinitionManager.AlterTypeDefinition("Order", type => type
                .DisplayedAs("Order")
                .WithPart("Order")
                .WithPart("OrderPart")
                .WithPart("CommonPart", p => p.WithSetting("OwnerEditorSettings.ShowOwnerEditor", "false"))
                .WithPart("IdentityPart"));

            return 2;
        }

        public int UpdateFrom2() {
            SchemaBuilder.AlterTable("OrderPartRecord", table => table
                .AddColumn<int>("UserId", column => column.WithDefault(-1)));
            return 3;
        }

        public int UpdateFrom3() {

            //get all existing orders that do not have a CurrencyCode set, and set that to the currency
            //corresponding to the one set in the active ICurrencyProvider
            var query = _contentManager
                .Query<OrderPart, OrderPartRecord>(VersionOptions.AllVersions)
                .Where(opr => !opr.Contents.Contains("currencyCode"));
            string code = _currencyProvider.CurrencyCode;
            foreach (var item in query.List()) {
                item.CurrencyCode = code;
            }
            return 4;
        }
    }
}
