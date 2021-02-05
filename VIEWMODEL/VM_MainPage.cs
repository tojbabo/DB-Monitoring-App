using LiveCharts;
using LiveCharts.Wpf;
using MONITOR_APP.MODEL;
using MONITOR_APP.UTILITY;
using MONITOR_APP.VIEW;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace MONITOR_APP.VIEWMODEL
{

    public class VM_MainPage
    {
        BASE head;
        SelectOptWindow SOW;

        public ObservableCollection<SearchData> Searches { get; set; }
        public ObservableCollection<ChartData> Vms { get; set; }
        //public ChartValues<CustomVm> cs { get; set; }

        public VM_MainPage()
        {
            head = BASE.getBASE();
            Vms = new ObservableCollection<ChartData>();
            Searches = new ObservableCollection<SearchData>();
            SOW = null;


        }

        #region chart
        // TABLE을 통해 CAHRT를 생성하는 함수
        void CreateChart(object opt)
        {
            string[] options = (string[])opt;
            string table = options[0];
            string danji = options[1];
            string build = options[2];
            string house = options[3];
            string room = options[4];
            bool curtmp = Convert.ToBoolean(options[5]);
            bool settmp = Convert.ToBoolean(options[6]);
            bool onff = Convert.ToBoolean(options[7]);

            #region 선언부
            ChartData chart = new ChartData();
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                Vms.Add(chart);
            }));

            DataTable dt = GetDataTable(options);
            ChartValues<double> set_temp = new ChartValues<double>();
            ChartValues<double> cur_temp = new ChartValues<double>();
            ChartValues<double> data_onoff = new ChartValues<double>();
            #endregion

            #region 데이터 저장부
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                foreach (DataRow r in dt.Rows)
                {
                    set_temp.Add(Convert.ToDouble(r["SET_TEMP"].ToString()) / 10);
                    cur_temp.Add(Convert.ToDouble(r["CUR_TEMP"].ToString()) / 10);
                    data_onoff.Add((bool)r["OP_ONOFF"] ? 1 : 0);
                    chart.Labels.Add(TimeConverter.GetTime(r["TIME"].ToString()));
                }


                if (cur_temp.Count != 0)
                {
                    chart.AxisYCollection.Add(new LiveCharts.Wpf.Axis { Title = "temporature", Foreground = Brushes.Blue, MinValue = -5, MaxValue = 35 });
                    chart.series.Add(new LineSeries
                    {
                        Title = "Current Temp",
                        Values = cur_temp,
                        LineSmoothness = 0, //0: straight lines, 1: really smooth lines
                        PointGeometry = null,
                        ScalesYAt = 0,
                    });
                }

                if (set_temp.Count != 0)
                {
                    if (chart.AxisYCollection.Count == 0)
                        chart.AxisYCollection.Add(new LiveCharts.Wpf.Axis { Title = "temporature", Foreground = Brushes.Blue, MinValue = -5, MaxValue = 35 });
                    chart.series.Add(new LineSeries
                    {
                        Title = "Setting Temp",
                        Values = set_temp,
                        LineSmoothness = 0, //0: straight lines, 1: really smooth lines
                        PointGeometry = null,
                        ScalesYAt = 0,
                    });
                }

                if (data_onoff.Count != 0)
                {
                    chart.AxisYCollection.Add(new LiveCharts.Wpf.Axis { Title = "on/off", Foreground = Brushes.Gold, MinValue = -1, MaxValue = 10 });
                    chart.series.Add(new LineSeries
                    {
                        Title = "ON / OFF",
                        Values = data_onoff,
                        LineSmoothness = 1, //0: straight lines, 1: really smooth lines
                        PointGeometry = null,
                        ScalesYAt = 1,
                    });
                }

                chart.title = $"{danji} - {build} - {house} - {room}";
                chart.selected = false;
                chart.searches = new SearchData
                {
                    danji = danji,
                    build = build,
                    house = house,
                    room = room,

                    tmp_cur = curtmp,
                    tmp_set = settmp,
                    on_off = onff
                };

                ChartRedraw(chart);
            }));
            #endregion
        }
        // 생성된 모든 CHART를 지우는 함수
        public void ChartReset()
        {
            Vms.Clear();
        }
        // 선택 옵션에 따라 그래프 출력을 변경하는 함수
        public void ChartRedraw(ChartData chart)
        {
            foreach (LineSeries d in chart.series)
            {
                if (d.Title == "Current Temp") d.Visibility = (chart.searches.tmp_cur == true) ? Visibility.Visible : Visibility.Hidden;
                else if (d.Title == "Setting Temp") d.Visibility = (chart.searches.tmp_set == true) ? Visibility.Visible : Visibility.Hidden;
                else if (d.Title == "ON / OFF") d.Visibility = (chart.searches.on_off == true) ? Visibility.Visible : Visibility.Hidden;
            }
        }

        #endregion

        #region SQL
        // TABLE 내 검색 조건 미리 보기 생성
        public void CreateSearchData()
        {
            var conn = head.getConnect();
            while (conn == null)
            {
                Thread.Sleep(500);
                conn = head.getConnect();
            }

            string query = MySQL.SearchQuery("sensor_data");

            var table = MySQL.SelectTable(conn, query);
            if (table == null) return;

            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                foreach (DataRow r in table.Rows)
                {
                    Searches.Add(new SearchData()
                    {
                        danji = r["DANJI_ID"].ToString(),
                        build = r["BUILD_ID"].ToString(),
                        house = r["HOUSE_ID"].ToString(),
                        room = r["ROOM_ID"].ToString(),
                    });
                }
            }));

        }
        // 검색 조건 설정 윈도우 생성
        public void RequestSelect(string opts = null)
        {
            if (SOW == null)
            {
                SOW = new SelectOptWindow();

                SOW.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                SOW.Topmost = true;
                SOW.OnChildTextInputEvent += new SelectOptWindow.OnChildTextInputHandler(SOW_OnChildTextInputEvent);
                SOW.Show();

            }
            else
            {
                SOW.Focus();
            }

            if (opts != null) SOW.SetData(opts);
                
            
        }
        // 검색 조건 설정 윈도우 응답 함수
        void SOW_OnChildTextInputEvent(string Parameters)
        {
            if (Parameters != null)
            {
                string[] options = Parameters.Split('\\');

                if (SOW != null)
                {
                    SOW.Close();
                    SOW.OnChildTextInputEvent -= new SelectOptWindow.OnChildTextInputHandler(SOW_OnChildTextInputEvent);
                    SOW = null;
                }

                Thread t = new Thread(new ParameterizedThreadStart(CreateChart));
                t.Start(options);
                //CreateChart(options);
            }
            SOW = null;
        }
        // 입력 조건을 기반으로 SQL 서버로부터 TABLE 얻어오는 함수
        DataTable GetDataTable(string[] opts)
        {
            var conn = head.getConnect();

            DataTable dt = MySQL.SelectTable(conn,MySQL.MakeQuery(opts));

            if (dt == null) MessageBox.Show("data table is null");

            return dt;

        }
        #endregion
        
    }
}