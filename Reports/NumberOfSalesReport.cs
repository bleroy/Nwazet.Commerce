using System.Collections.Generic;
using System.Linq;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Models.Reporting;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Reports {
    [OrchardFeature("Nwazet.Reports")]
    public class NumberOfSalesReport : BaseSalesReport {
        public NumberOfSalesReport(IContentManager contentManager) : base(contentManager) {
        }

        public override string Name {
            get { return T("Number of orders").Text; }
        }

        public override string Description {
            get { return T("Number of distinct successful orders").Text; }
        }

        public override string DescriptionColumnHeader {
            get { return T("Period").Text; }
        }

        public override string ValueColumnHeader {
            get { return T("Number of orders").Text; }
        }

        public override string ValueFormat {
            get { return null; }
        }

        public override ChartType ChartType {
            get { return ChartType.Line; }
        }

        public override decimal ComputeResultForInterval(IList<OrderPart> ordersForInterval) {
            return ordersForInterval.Count();
        }
    }
}
