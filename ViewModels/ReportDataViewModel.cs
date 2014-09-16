using System.Collections.Generic;
using Nwazet.Commerce.Models.Reporting;
using Orchard.Core.Common.ViewModels;

namespace Nwazet.Commerce.ViewModels {
    public class ReportDataViewModel {
        public IEnumerable<ReportDataPoint> DataPoints { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DescriptionColumnHeader { get; set; }
        public string ValueColumnHeader { get; set; }
        public string ValueFormat { get; set; }
        public ChartType ChartType { get; set; }
        public string Preset { get; set; }
        public DateTimeEditor StartDateEditor { get; set; }
        public DateTimeEditor EndDateEditor { get; set; }
        public TimePeriod Granularity { get; set; }
        public IEnumerable<string> Series { get; set; } 
    }
}
