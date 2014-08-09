using System;
using System.Collections.Generic;
using Orchard;

namespace Nwazet.Commerce.Models.Reporting {
    public interface ICommerceReport : IDependency {
        string Name { get; }
        string Description { get; }
        string DescriptionColumnHeader { get; }
        string ValueColumnHeader { get; }
        string ValueFormat { get; }
        ChartType ChartType { get; }

        IEnumerable<ReportDataPoint> GetData(
            DateTime startDate, 
            DateTime endDate,
            TimePeriod granularity);
    }
}
