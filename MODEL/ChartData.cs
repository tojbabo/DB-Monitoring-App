using LiveCharts;
using LiveCharts.Wpf;
using MONITOR_APP.UTILITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MONITOR_APP.MODEL
{
    public class ChartData
    {

        public ChartValues<double> set_tmp;
        public ChartValues<double> cur_tmp;
        public ChartValues<double> onff;
        public string title { get; set; }
        public SeriesCollection series { get; set; }
        public AxesCollection AxisYCollection { get; set; }
        public List<string> Labels { get; set; }
        public List<string> _labes { get; set; }
        public bool selected { get; set; }
        public SearchData searches { get; set; }

        public ChartData()
        {
            series = new SeriesCollection();
            AxisYCollection = new AxesCollection();
            Labels = new List<string>();
            searches = new SearchData();

            set_tmp = new ChartValues<double>();
            cur_tmp = new ChartValues<double>();
            onff = new ChartValues<double>();
        }

        public void ReFresh()
        {
            AxisYCollection.Clear();
            series.Clear();

            Drawing();
        }

        public void Drawing()
        {
            if (searches.tmp_cur)
            {
                AxisYCollection.Add(new LiveCharts.Wpf.Axis { Title = "temporature",  MinValue = -5, MaxValue = 35 });

                series.Add(new LineSeries
                {
                    Title = "Current Temp",
                    Values = cur_tmp,
                    LineSmoothness = 1, //0: straight lines, 1: really smooth lines
                    PointGeometry = null,
                    ScalesYAt = 0,
                    Fill = new SolidColorBrush(Colors.Transparent),
                    Stroke = new SolidColorBrush(Colors.Blue),
                    
                });
            }

            if (searches.tmp_set)
            {
                if (AxisYCollection.Count == 0)
                    AxisYCollection.Add(new LiveCharts.Wpf.Axis { Title = "temporature", MinValue = -5, MaxValue = 35 });

                series.Add(new LineSeries
                {
                    Title = "Setting Temp",
                    Values = set_tmp,
                    LineSmoothness = 1, //0: straight lines, 1: really smooth lines
                    PointGeometry = null,
                    ScalesYAt = 0,
                    Fill = new SolidColorBrush(Colors.Transparent),
                    Stroke = new SolidColorBrush(Colors.Red),
                });
            }

            if (searches.on_off)
            {
                AxisYCollection.Add(new LiveCharts.Wpf.Axis {Foreground = Brushes.Transparent, MinValue = -1, MaxValue = 10 });

                series.Add(new LineSeries
                {
                    Title = "ON / OFF",
                    Values = onff,
                    LineSmoothness = 0, //0: straight lines, 1: really smooth lines
                    PointGeometry = null,
                    ScalesYAt = (AxisYCollection.Count==1)?0:1,
                    Fill = new SolidColorBrush(Color.FromArgb(120,219,255,171)),
                    Stroke = new SolidColorBrush(Colors.Green),
                });
            }
        }
    }
}