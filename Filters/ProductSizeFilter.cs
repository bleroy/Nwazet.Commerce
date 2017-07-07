using System;
using Nwazet.Commerce.Extensions;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Projections.Descriptors.Filter;
using Orchard.Projections.FilterEditors.Forms;
using Orchard.Projections.Services;

namespace Nwazet.Commerce.Filters {
    public class ProductSizeFilter : IFilterProvider {

        public Localizer T { get; set; }

        public ProductSizeFilter() {
             T = NullLocalizer.Instance;
        }

        public void Describe(DescribeFilterContext describe) {
            describe.For("ProductPart", T("Product"), T("Product"))
                .Element("ProductSize", T("Product Size"), T("Product Size Filter."),
                    (Action<dynamic>)ApplyFilter,
                    (Func<dynamic, LocalizedString>)DisplayFilter,
                    "ProductSizeFilterForm"
                );
        }

        public void ApplyFilter(dynamic context) {
            string value = context.State.ProductFormSize;
            var op = (StringOperator)Enum.Parse(typeof(StringOperator), Convert.ToString(context.State.Operator));
            var filterExpression = FilterHelper.GetFilterPredicateString(op, "Size", value);
            var query = (IHqlQuery)context.Query;
            context.Query = query
                .Where(x => x.ContentPartRecord<ProductPartVersionRecord>(), filterExpression);
            return;
        }

        public LocalizedString DisplayFilter(dynamic context) {
            return FilterHelper.DisplayFilterString(T, context.State, "Product Size");
        }
    }
}