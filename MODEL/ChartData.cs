
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
        #region 그래프 그리기
        // 그래프 자체 MODEL
        public PlotModel vm { get; set; }
        // 그래프 타이틀
        public string title { get; set; }
        // 설정 온도 그래프 데이터 (X, Y)
        public List<DataPoint> set { get; set; }
        // 현재 온도 그래프 데이터 (X, Y)
        public List<DataPoint> cur { get; set; }
        // 벨브 ON/OFF 그래프 데이터 (X, Y)
        public List<DataPoint> onff { get; set; }
        // 조절기 MODE별 영역 지정
        public List<RectangleAnnotation> Rectangles;
        
        // 그래프 마우스 특정 지점 세부 데이터 출력 model
        public PlotController plt { get; set; }
        #endregion

        #region 그래프 세부 사항
        // 그래프 검색 조건 
        public SearchData searches { get; set; }
        // 검색 시간(유닉스 시간)을 시:분:초 형식으로 변환 가져옴, 반대 셋팅 :: 실 데이터는 searches에 존재
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
        // 그래프 내 데이터 개수
        public int Count
        {
            get
            {
                return set.Count();
            }
        }
        // 해당 그래프 선택됨(안됨) - 그래프 내 숨겨진 그리드 출력 여부 
        public bool selected { get; set; }
        // 벨브 개방시간 총합
        public double onfftime { get; set; }
        // 벨브 총 개방 시간 시분초 형식 변환
        public TimeSpan onofftime
        {
            get
            {
                return TimeSpan.FromSeconds(onfftime);
            }
        }
        // 벨브 개방 개수 
        public double onffcount { get; set; }
        // 벨브 개방 비율
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
        // 평균 기온
        public string avgtmp { 
            get
            {
                return string.Format("{0:0.0}", cur.Sum(x => x.Y)/cur.Count());
            } 
        }

        #endregion

        #region 그래프 설정 데이터
        public int ymin;
        public int ymax;

        public int onvalue;
        public int offvalue;
        #endregion

        #region 메서드

        public ChartData(StaticValue sv)
        {
            plt = new PlotController();
            //plt.UnbindAll();

            ymin = sv.AXISYMIN;
            ymax = sv.AXISYMAX;
            onvalue = sv.ONVALUE;
            offvalue = sv.OFFVALUE;


            vm = new PlotModel()
            {
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.TopCenter,
                LegendOrientation = LegendOrientation.Horizontal,
            };
            set = new List<DataPoint>();
            cur = new List<DataPoint>();
            onff = new List<DataPoint>();
            searches = new SearchData();
            Rectangles = new List<RectangleAnnotation>();
        }

        public void ReFresh()
        {
            vm.Annotations.Clear();
            vm = new PlotModel()
            {
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.TopCenter,
                LegendOrientation = LegendOrientation.Horizontal,
            };
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
                Minimum = ymin,
                Maximum = ymax,
                AbsoluteMinimum = ymin,
                AbsoluteMaximum = ymax,
            }) ;

            title = $"ROOM {searches.ROOM_ID}";

            if(cur.Count!=0) vm.Series.Add(new LineSeries
            {
                TrackerFormatString = "{0}\n온도: {4}℃\n시간: {2}",
                Title = "현재 온도",
                ItemsSource = cur,
                StrokeThickness = 2,
                Color = OxyColors.Black,
            });

            if (set.Count != 0) vm.Series.Add(new StairStepSeries
            {
                TrackerFormatString = "{0}\n온도: {4}℃\n시간: {2}",
                Title = "설정 온도",
                ItemsSource = set,
                StrokeThickness = 2,
                Color = OxyColors.Red,
            });
            
            if(onff.Count!=0) vm.Series.Add(new StairStepSeries
            {
                TrackerFormatString = "{0}\n시간: {2}",
                Title = "벨브 상태",
                ItemsSource = onff,
                StrokeThickness = 2,
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