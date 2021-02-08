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

        public double min = 0;
        public double max = 0;

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
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                foreach (DataRow r in dt.Rows)
                {
                    try
                    {
                        set = Convert.ToDouble(r["SET_TEMP"].ToString()) / 10;
                        cur = Convert.ToDouble(r["CUR_TEMP"].ToString()) / 10;
                        onoff = (bool)r["VALVE_STATUS"] ? 1 : 0;
                        time = TimeConverter.GetTime(r["TIME"].ToString());
                    }
                    catch(Exception e)
                    {
                        continue;
                    }


                    chart.set_tmp.Add(Convert.ToDouble(r["SET_TEMP"].ToString()) / 10);
                    chart.cur_tmp.Add(Convert.ToDouble(r["CUR_TEMP"].ToString()) / 10);
                    chart.onff.Add((bool)r["OP_ONOFF"] ? 1 : 0);
                    chart.Labels.Add(TimeConverter.GetTime(r["TIME"].ToString()));
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

                //ChartRedraw(chart);
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

            min = 1606271503-1;

            table = MySQL.SelectTable(conn, "SELECT TIME FROM sensor_data ORDER BY ID DESC LIMIT 1;");
            if (table == null) return;

            max = Convert.ToDouble(table.Rows[0]["TIME"].ToString())-1;

        }
        // 검색 조건 설정 윈도우 생성
        public void RequestSelect(string opts = null)
        {
            string time = $"{min}\\{max}";
            if (SOW == null)
            {
                SOW = new SelectOptWindow(time);

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

            DataTable dt = MySQL.SelectTable(conn, MySQL.MakeQuery(opts));

            if (dt == null) MessageBox.Show("data table is null");

            return dt;

        }
        #endregion

    }
}