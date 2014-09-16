using System.Collections.Generic;

namespace Nwazet.Commerce.Models.Reporting {
    public class ReportData {
        public IEnumerable<ReportDataPoint> DataPoints { get; set; }
        public IEnumerable<string> Series { get; set; } 
    }
}
