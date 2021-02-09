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
            int amount = Convert.ToInt32(options[11]);

            #region 선언부
            ChartData chart = new ChartData();
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                Vms.Add(chart);
            }));

            DataTable dt = GetDataTable(options);
            Console.WriteLine($"dt count is :{dt.Rows.Count}");

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("데이터를 찾을 수 없습니다.");
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    Vms.Remove(chart);
                }));
                return;
            }


            ChartValues<double> set_temp = new ChartValues<double>();
            ChartValues<double> cur_temp = new ChartValues<double>();
            ChartValues<double> data_onoff = new ChartValues<double>();
            double set;
            double cur;
            double onoff;
            string time;
            #endregion

            #region 데이터 저장부
            int i = 0;
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                foreach (DataRow r in dt.Rows)
                {
                    if (i == amount) break;
                    i++;
                    try
                    {
                        set = Convert.ToDouble(r["SET_TEMP"].ToString()) / 10;
                        cur = Convert.ToDouble(r["CUR_TEMP"].ToString()) / 10;
                        onoff = (bool)r["VALVE_STATUS"] ? 1 : 0;
                        time = TimeConverter.GetAll(r["TIME"].ToString());
                    }
                    catch(Exception e)
                    {
                        continue;
                    }


                    chart.set_tmp.Add(set);
                    chart.cur_tmp.Add(cur);
                    chart.onff.Add(onoff);
                    chart.Labels.Add(time);
                }

                chart.title = $"{danji} - {build} - {house} - {room}";
                chart.amount = amount;
                chart.minday = Convert.ToInt64(options[8]);
                chart.maxday = Convert.ToInt64(options[9]);
                chart.interval = Convert.ToInt32(options[10]);

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

                chart.Drawing();
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

            DataTable dt = MySQL.SelectTable(conn, MySQL.MakeQuery(opts));

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


            var table = MySQL.SelectTable(conn, minquery);

            double min = Convert.ToDouble(table.Rows[0]["TIME"].ToString());

            table = MySQL.SelectTable(conn, maxquery);

            double max = Convert.ToDouble(table.Rows[0]["TIME"].ToString()) + 1;


            return (min, max);

        }

    }
}