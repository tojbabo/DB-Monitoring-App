using LiveCharts;
using MONITOR_APP.UTILITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MONITOR_APP.MODEL
{
    public class ChartData
    {
        public ChartValues<CustomVm> Vm { get; set; }
        public ChartValues<double> data { get; set; }
        public List<string> Labels { get; set; }

        public ChartData()
        {
            data = new ChartValues<double>();
            Vm = new ChartValues<CustomVm>();
            Labels = new List<string>();
        }
    }
}
