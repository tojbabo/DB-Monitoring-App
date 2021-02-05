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
        public string title { get; set; }
        public SeriesCollection series { get; set; }
        public AxesCollection AxisYCollection { get; set; }
        public List<string> Labels { get; set; }
        public bool selected { get; set; }
        public SearchData searches { get; set; }


        public ChartData()
        {
            series = new SeriesCollection();
            AxisYCollection = new AxesCollection();
            Labels = new List<string>();
            searches = new SearchData();
        }
        public ChartData(ChartData cdata)
        {
            title = cdata.title;
            series = new SeriesCollection();
            foreach (LineSeries d in cdata.series)
            {
                series.Add(new LineSeries
                {
                    Title = d.Title,
                    Values = d.Values,
                    PointGeometry = d.PointGeometry,
                    ScalesYAt = d.ScalesYAt,
                    LineSmoothness = d.LineSmoothness,
                });
            }
            AxisYCollection = new AxesCollection();
            foreach (var d in cdata.AxisYCollection)
            {
                AxisYCollection.Add(new Axis
                {
                    Title = d.Title,
                    Foreground = d.Foreground,
                    MinValue = d.MinValue,
                    MaxValue = d.MaxValue,
                });
            }
            Labels = new List<string>(cdata.Labels);
            selected = cdata.selected;
            searches = new SearchData(cdata.searches);
        }
        public void HardCopy(ChartData cdata)
        {
            title = cdata.title;
            foreach (LineSeries d in cdata.series)
            {
                series.Add(new LineSeries
                {
                    Visibility = d.Visibility,
                    Title = d.Title,
                    Values = d.Values,
                    PointGeometry = d.PointGeometry,
                    ScalesYAt = d.ScalesYAt,
                    LineSmoothness = d.LineSmoothness,
                });
            }
            foreach (var d in cdata.AxisYCollection)
            {
                AxisYCollection.Add(new Axis
                {
                    Title = d.Title,
                    Foreground = d.Foreground,
                    MinValue = d.MinValue,
                    MaxValue = d.MaxValue,
                });
            }
            Labels = new List<string>(cdata.Labels);
            selected = cdata.selected;
            searches = new SearchData(cdata.searches);
        }
        public void ReFresh()
        {
            SeriesCollection newSeries = new SeriesCollection();
            foreach (LineSeries d in series)
            {
                LineSeries l = new LineSeries();
                double[] a = new double[d.Values.Count];
                d.Values.CopyTo(a, 0);
                ChartValues<double> dd = new ChartValues<double>(a);
                
                newSeries.Add(new LineSeries
                {
                    Title = d.Title,
                    Values = dd,
                    PointGeometry = d.PointGeometry,
                    ScalesYAt = d.ScalesYAt,
                    LineSmoothness = d.LineSmoothness,
                });
            }
            series.Clear();
            series = newSeries;

            AxesCollection newAxes = new AxesCollection();
            foreach (var d in AxisYCollection)
            {
                newAxes.Add(new Axis
                {
                    Title = d.Title,
                    Foreground = d.Foreground,
                    MinValue = d.MinValue,
                    MaxValue = d.MaxValue,
                });
            }
            AxisYCollection.Clear();
            AxisYCollection = newAxes;
        }
    }
}