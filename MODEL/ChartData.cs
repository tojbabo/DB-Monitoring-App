
using MONITOR_APP.UTILITY;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MONITOR_APP.MODEL
{
    public class ChartData
    {
        public PlotModel vm { get; set; }
        public List<DataPoint> set { get; set; }
        public List<DataPoint> cur { get; set; }
        public List<DataPoint> onff { get; set; }
        public List<RectangleAnnotation> Rectangles;
        public bool selected { get; set; }
        public SearchData searches { get; set; }

        public DateTime lilday
        {
            get
            {
                return TimeConverter.ConvertTimestamp(searches.mintime);
            }
            set
            {
                searches.mintime = ((DateTimeOffset)value).ToUnixTimeSeconds();
            }
        }
        public DateTime bigday
        {
            get
            {
                return TimeConverter.ConvertTimestamp(searches.maxtime);
            }
            set
            {
                searches.maxtime = ((DateTimeOffset)value).ToUnixTimeSeconds();
            }
        }
        public string title { get; set; }
        public int Count { 
            get
            {
                return set.Count();
            }}
        public PlotController plt { get; set; }



        public double onfftime { get; set; }
        public TimeSpan onofftime
        {
            get
            {
                return TimeSpan.FromSeconds(onfftime);
            }
        }
        public double onffcount { get; set; }
        public string onffratio { 
            get
            {
                return string.Format("{0:0.0}", (onffcount / onff.Count())*100);
            }
            set
            {
                onffratio = value;
            }
        }
        public string avgtmp { 
            get
            {
                return string.Format("{0:0.0}", cur.Sum(x => x.Y)/cur.Count());
            } 
        }


        #region 메서드

        public ChartData()
        {
            plt = new PlotController();
            //plt.UnbindAll();


            vm = new PlotModel();
            set = new List<DataPoint>();
            cur = new List<DataPoint>();
            onff = new List<DataPoint>();
            searches = new SearchData();
            Rectangles = new List<RectangleAnnotation>();
        }

        public void ReFresh()
        {
            vm.Annotations.Clear();
            vm = new PlotModel();
            Drawing();
        }

        public void Drawing()
        {
            vm.Axes.Add(new DateTimeAxis()
            {
                Position = AxisPosition.Bottom,
                //IsZoomEnabled = false,
                //IsPanEnabled = false,

                AbsoluteMinimum = (set.Count != 0) ? set.First().X : double.MinValue,
                AbsoluteMaximum = (set.Count != 0) ? set.Last().X : double.MaxValue,
            }) ;
            vm.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                //IsZoomEnabled = false,
                //IsPanEnabled = false,
                Minimum = -5,
                Maximum = 40,
                AbsoluteMinimum = -5,
                AbsoluteMaximum = 40,
            }) ;

            title = $"ROOM {searches.ROOM_ID}";

            if (set.Count!=0) vm.Series.Add(new StairStepSeries
            {
                //TrackerFormatString = "x={2},\ny={4}",
                Title = "set",
                ItemsSource = set,
                StrokeThickness = 2,
                LineStyle = LineStyle.Solid,
                Color = OxyColors.Red,
            });
            
            if(cur.Count!=0) vm.Series.Add(new StairStepSeries
            {
                Title = "cur",
                ItemsSource = cur,
                StrokeThickness = 2,
                LineStyle = LineStyle.Solid,
                Color = OxyColors.Black,
            });
            
            if(onff.Count!=0) vm.Series.Add(new StairStepSeries
            {
                Title = "onff",
                ItemsSource = onff,
                StrokeThickness = 2,
                LineStyle = LineStyle.Solid,
                Color = OxyColors.Green,
            });

            foreach(var v in Rectangles)
            {
                vm.Annotations.Add(v);
            }

        }

        #endregion

    }

}