using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Models.Reporting;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Environment.Extensions;
using Orchard.Localization;

namespace Nwazet.Commerce.Reports {
    [OrchardFeature("Nwazet.Reports")]
    public class NumberOfSalesReport : ICommerceReport {
        private readonly IContentManager _contentManager;

        public NumberOfSalesReport(IContentManager contentManager) {
            _contentManager = contentManager;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public string Name {
            get { return T("Number of sales").Text; }
        }

        public string Description {
            get { return T("Number of sales").Text; }
        }

        public string DescriptionColumnHeader { get { return T("Period").Text; } }
        public string ValueColumnHeader { get { return T("Number of sales").Text; } }

        public ChartType ChartType { get { return ChartType.Line; } }

        public IEnumerable<ReportDataPoint> GetData(DateTime startDate, DateTime endDate, TimePeriod granularity) {
            startDate = granularity.BeginningDate(startDate);
            endDate = granularity.EndingDate(endDate);
            var orders = _contentManager
                .Query<CommonPart, CommonPartRecord>("Order")
                .Where(r =>
                    r.CreatedUtc >= startDate.ToUniversalTime()
                    && r.CreatedUtc <= endDate.ToUniversalTime())
                .OrderBy(r => r.CreatedUtc)
                .Join<OrderPartRecord>()
                .Where(order => order.Status != OrderPart.Cancelled)
                .List()
                .ToList();
            var numberOfPoints = granularity.PeriodCount(startDate, endDate);
            var results = new List<ReportDataPoint>(numberOfPoints);
            var intervalStart = startDate;
            var intervalEnd = startDate + granularity;
            while (intervalStart < endDate) {
                var count = orders.Count(
                    order => order.CreatedUtc >= intervalStart
                             && order.CreatedUtc < intervalEnd);
                results.Add(new ReportDataPoint {
                    Description = granularity.ToString(intervalStart, CultureInfo.CurrentUICulture),
                    Value = count
                });
                intervalStart = intervalEnd;
                intervalEnd = intervalStart + granularity;
            }
            return results;
        }
    }
}
