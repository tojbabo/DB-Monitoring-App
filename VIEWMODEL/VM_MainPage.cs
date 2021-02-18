
using MONITOR_APP.MODEL;
using MONITOR_APP.UTILITY;
using MONITOR_APP.VIEW;
using OxyPlot;
using OxyPlot.Axes;
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
            SearchData data = (SearchData)opt;

            #region 선언부
            ChartData chart = new ChartData();

            DataTable dt = GetDataTable(data);


            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("데이터를 찾을 수 없습니다.");
                
                return;
            }

            double set, cur, onoff,time;
            #endregion

            #region 데이터 저장부
            int i = 0;
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                foreach (DataRow r in dt.Rows)
                {
                    if (i == data.amount) break;
                    i++;
                    try
                    {
                        set = Convert.ToDouble(r["SET_TEMP"].ToString()) / 10;
                        cur = Convert.ToDouble(r["CUR_TEMP"].ToString()) / 10;
                        onoff = (bool)r["VALVE_STATUS"] ? 3 : 0;
                        time = DateTimeAxis.ToDouble(TimeConverter.ConvertTimestamp(Convert.ToDouble(r["TIME"].ToString())));
                    }
                    catch(Exception e)
                    {
                        continue;
                    }

                    chart.onff.Add(new DataPoint(time, onoff));
                    chart.set.Add(new DataPoint(time, set));
                    chart.cur.Add(new DataPoint(time, cur));
                }

                chart.Count = i;

                chart.selected = false;
                chart.searches = data;

                chart.Drawing();
                Vms.Add(chart);
            }));

            #endregion
        }
        // 생성된 모든 CHART를 지우는 함수
        public void ChartReset()
        {
            Vms.Clear();
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

            string query = DB_mysql.SearchQuery("sensor_data");

            var table = DB_mysql.SelectTable(conn, query);
            if (table == null) return;

            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                foreach (DataRow r in table.Rows)
                {
                    Searches.Add(new SearchData()
                    {
                        DANJI_ID = r["DANJI_ID"].ToString(),
                        BUILD_ID = r["BUILD_ID"].ToString(),
                        HOUSE_ID = r["HOUSE_ID"].ToString(),
                        ROOM_ID = r["ROOM_ID"].ToString(),
                    });
                }
            }));
        }
        // 검색 조건 설정 윈도우 생성
        public void RequestSelect(string opts = null)
        {
            double mintime;
            double maxtime;
            if (SOW == null)
            {
                (mintime, maxtime) = Getminmax(opts);

                SOW = new SelectOptWindow();
                SOW.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                SOW.Topmost = true;
                SOW.OnChildTextInputEvent += new SelectOptWindow.OnChildTextInputHandler(SOW_OnChildTextInputEvent);
                SOW.Show();
            }
            else
            {
                (mintime, maxtime) = Getminmax(opts);

                SOW.Focus();
            }

            
            string time = $"{mintime}\\{maxtime}";
            if (opts != null)
            {
                SOW.SetData($"{time}\\{opts}");
            }
            else
                SOW.SetData(time);


        }
        // 검색 조건 설정 윈도우 응답 함수
        void SOW_OnChildTextInputEvent(object Parameters)
        {
            if (Parameters != null)
            {
                if (SOW != null)
                {
                    SOW.Close();
                    SOW.OnChildTextInputEvent -= new SelectOptWindow.OnChildTextInputHandler(SOW_OnChildTextInputEvent);
                    SOW = null;
                }

                Thread t = new Thread(new ParameterizedThreadStart(CreateChart));
                t.Start(Parameters);
                //CreateChart(options);
            }
            SOW = null;
        }
        // 입력 조건을 기반으로 SQL 서버로부터 TABLE 얻어오는 함수
        DataTable GetDataTable(object obj)
        {
            var conn = head.getConnect();

            DataTable dt = DB_mysql.SelectTable(conn, DB_mysql.MakeQuery(obj));

            if (dt == null) MessageBox.Show("data table is null");

            return dt;

        }
        #endregion
        (double,double) Getminmax(string datas)
        {
            if (datas == null)
                return (-1,-1);
            var conn = head.getConnect();

            string[] opts = datas.Split('\\');

            string danji = opts[0];
            string build = opts[1];
            string house = opts[2];
            string room = opts[3];

            string query = $"SELECT * FROM {"sensor_data"} WHERE ID IS NOT NULL ";

            if (danji != "") danji = $" AND DANJI_ID = '{danji}'";
            if (build != "") build = $" AND BUILD_ID = '{build}'";
            if (house != "") house = $" AND HOUSE_ID = '{house}'";
            if (room != "") room = $" AND ROOM_ID = '{room}'";

            string minquery = query + danji + build + house + room + " LIMIT 1;";
            string maxquery = query + danji + build + house + room + " ORDER BY ID DESC LIMIT 1;";


            var table = DB_mysql.SelectTable(conn, minquery);

            double min = Convert.ToDouble(table.Rows[0]["TIME"].ToString());

            table = DB_mysql.SelectTable(conn, maxquery);

            double max = Convert.ToDouble(table.Rows[0]["TIME"].ToString()) + 1;


            return (min, max);

        }

    }
}