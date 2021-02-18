
using MONITOR_APP.UTILITY;
using OxyPlot;
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

        public bool selected { get; set; }
        public SearchData searches { get; set; }

        // 테스트 프로퍼티
        public string minday
        {
            get
            {
                return $"{TimeConverter.GetDate(searches.mintime)}";
            }
        }
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
        public string maxday
        { 
            get
            {
                return $"{TimeConverter.GetDate(searches.maxtime)}";
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

        public int Count { get; set; }

        public bool isComboOpen = false;

        public PlotController plt { get; set; }

        #region 메서드

        public ChartData()
        {
            plt = new PlotController();
            plt.UnbindAll();


            vm = new PlotModel();
            set = new List<DataPoint>();
            cur = new List<DataPoint>();
            onff = new List<DataPoint>();
            searches = new SearchData();
        }

        public void ReFresh()
        {
            vm = new PlotModel();
            Drawing();
        }

        public void Drawing()
        {
            vm.Title = $"Room: {this.searches.ROOM_ID}";
            

            vm.Axes.Add(new DateTimeAxis()
            {
                
                Position = AxisPosition.Bottom,
                IsZoomEnabled = false,
                IsPanEnabled = false,

                //AbsoluteMinimum = minday,
                //AbsoluteMaximum = maxday,
            }) ;
            vm.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                IsZoomEnabled = false,
                IsPanEnabled = false,
                Minimum = -5,
                Maximum = 40,
                AbsoluteMinimum = -5,
                AbsoluteMaximum = 40,
            }) ;

            if(searches.tmp_set) vm.Series.Add(new StairStepSeries
            {
                Title = "set",
                ItemsSource = set,
                StrokeThickness = 2,
                LineStyle = LineStyle.Solid,
                Color = OxyColors.Red,
            });
            
            if(searches.tmp_cur) vm.Series.Add(new StairStepSeries
            {
                Title = "cur",
                ItemsSource = cur,
                StrokeThickness = 2,
                LineStyle = LineStyle.Solid,
                Color = OxyColors.Black,
            });
            
            if(searches.on_off) vm.Series.Add(new StairStepSeries
            {
                Title = "onff",
                ItemsSource = onff,
                StrokeThickness = 2,
                LineStyle = LineStyle.Solid,
                Color = OxyColors.Green,
            });
        }

        #endregion
    }

}