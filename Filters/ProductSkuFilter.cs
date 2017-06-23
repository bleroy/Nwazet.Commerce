using System;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Projections.Descriptors.Filter;
using Orchard.Projections.FilterEditors.Forms;
using Orchard.Projections.Services;

namespace Nwazet.Commerce.Filters {
    public class ProductSkuFilter : IFilterProvider {

        public Localizer T { get; set; }

        public ProductSkuFilter() {
            T = NullLocalizer.Instance;
        }

        public void Describe(DescribeFilterContext describe) {
            describe.For("ProductPart", T("Product"), T("Product"))
                .Element("ProductSku", T("Product Sku"), T("Product Sku Filter."),
                    (Action<dynamic>)ApplyFilter,
                    (Func<dynamic, LocalizedString>)DisplayFilter,
                    "ProductSkuFilterForm"
                );
        }

        public void ApplyFilter(dynamic context) {
            string value = context.State.Value;
            var op = (StringOperator)Enum.Parse(typeof(StringOperator), Convert.ToString(context.State.Operator));
            var filterExpression = FilterHelper.GetFilterPredicateString(op, "Sku", value);
            var query = (IHqlQuery)context.Query;
            context.Query = query.Where(x => x.ContentPartRecord<ProductPartRecord>(), filterExpression);
            return;
        }

        public LocalizedString DisplayFilter(dynamic context) {
            return FilterHelper.DisplayFilterString(T, context.State, "Product Sku");
        }
    }
}