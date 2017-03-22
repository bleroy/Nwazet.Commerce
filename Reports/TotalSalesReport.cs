using System.Collections.Generic;
using System.Linq;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Models.Reporting;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Reports {
    [OrchardFeature("Nwazet.Reports")]
    public class TotalSalesReport : BaseSalesReport {
        public TotalSalesReport(IContentManager contentManager) : base(contentManager) {
        }

        public override string Name {
            get { return T("Sales").Text; }
        }

        public override string Description {
            get { return T("Total sale amount").Text; }
        }

        public override string DescriptionColumnHeader {
            get { return T("Period").Text; }
        }

        public override string ValueColumnHeader {
            get { return T("Amount").Text; }
        }

        public override string ValueFormat {
            get { return "c"; }
        }

        public override ChartType ChartType {
            get { return ChartType.Line; }
        }

        public override decimal ComputeResultForInterval(IList<OrderPart> ordersForInterval) {
            return ordersForInterval.Any()
                ? ordersForInterval.Sum(order => order.AmountPaid)
                : 0.0M;
        }
    }
}