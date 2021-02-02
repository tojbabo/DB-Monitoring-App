using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MONITOR_APP.MODEL
{
    public class ChartData2
    {
        public int id { get; set; }
        public SeriesCollection yData { get; set; }
        public string[] Labels { get; set; }
    }
}
