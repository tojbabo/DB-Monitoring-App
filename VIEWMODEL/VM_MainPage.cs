
using InfluxDB.Client;
using InfluxDB.Client.Core.Flux.Domain;
using MONITOR_APP.MODEL;
using MONITOR_APP.UTILITY;
using MONITOR_APP.VIEW;
using NodaTime;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
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
        private void CreateChart(object opt)
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

        private async void CreateChart_influx(object opt)
        {
            SearchData data = (SearchData)opt;
            ChartData chart = new ChartData();
            double set, cur, onoff, time;
            double aimode, premode=-1;
            int datanum = 0;

            double rect = -1;
            OxyColor c = OxyColors.White;

            InfluxDBClient client = head.getClient();

            var tables = await DB_influx.ExcuteInflux(client, DB_influx.GetQuery(data));

            if (tables.Count == 0)
            {
                MessageBox.Show("데이터를 찾을 수 없습니다.");
                return;
            }

            time = 0;
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                string mode = DB_influx.GetData(tables, "AI_MODE");
                if (mode == "0") chart.mode = "MANUAL";
                else if (mode == "1") chart.mode = "AI";
                else if (mode == "2") chart.mode = "ECO 1";
                else if (mode == "3") chart.mode = "ECO 2";
                else if (mode == "4") chart.mode = "ECO 3";
                else if (mode == "5") chart.mode = "ECO 4";

                tables.ForEach(table =>
                {
                    table.Records.ForEach(cell =>
                    {
                        Instant inst = (Instant)cell.GetTime();
                        string field = cell.GetField();
                        time = DateTimeAxis.ToDouble(TimeConverter.ConvertTimestamp(inst.ToUnixTimeSeconds()));

                        if (field == "SET_TEMP")
                        {
                            set = DB_influx.GetData(cell, "SET_TEMP");
                            if (!double.IsNaN(set))
                            {
                                chart.set.Add(new DataPoint(time, set / 10));
                                datanum++;
                                return;
                            }
                        }
                        else if (field == "CUR_TEMP")
                        {
                            cur = DB_influx.GetData(cell, "CUR_TEMP");
                            if (!double.IsNaN(cur))
                            {
                                chart.cur.Add(new DataPoint(time, cur / 10));
                                return;
                            }
                        }
                        else if (field == "VALVE_STATUS")
                        {
                            onoff = DB_influx.GetData(cell, "VALVE_STATUS");
                            if (!double.IsNaN(onoff))
                            {
                                chart.onff.Add(new DataPoint(time, (onoff == 1) ? 5 : 0));
                                return;
                            }
                        }
                        else if(field == "AI_MODE")
                        {
                            aimode = Convert.ToDouble(cell.GetValue());
                            Console.Write($" {aimode}");
                            if (premode == -1)
                            {
                                premode = aimode;
                                if (premode != 0)
                                    rect = time;
                            }
                            else if (premode != aimode)
                            {
                                if (premode == 1)  // ai mode
                                    c = OxyColor.FromArgb(50, 0, 210, 0);
                                else if (premode == 2)  // eco 1
                                    c = OxyColor.FromArgb(50, 0, 0, 210);
                                else if (premode == 3)  // eco 2
                                    c = OxyColor.FromArgb(50, 90, 0, 170);
                                else if (premode == 4)  // eco 3
                                    c = OxyColor.FromArgb(50, 0, 90, 130);
                                else if (premode == 5)  // eco 4
                                    c = OxyColor.FromArgb(50, 90, 0, 0);

                                if (rect == -1)
                                    rect = time;

                                else if (rect != -1)
                                {
                                    if (premode != 0)
                                    {
                                        chart.Rectangles.Add(new RectangleAnnotation()
                                        {
                                            Fill = c,
                                            MinimumX = rect,
                                            MaximumX = time,
                                        });
                                        rect = -1;
                                    }
                                }
                                premode = aimode;
                            }
                            //Console.Write($" {cell.GetValue()}");
                        }
                    });
                });
                if(rect != -1)
                {
                    if (premode == 1)  // ai mode
                        c = OxyColor.FromArgb(50, 0, 210, 0);
                    else if (premode == 2)  // eco 1
                        c = OxyColor.FromArgb(50, 0, 0, 210);
                    else if (premode == 3)  // eco 2
                        c = OxyColor.FromArgb(50, 0, 0, 170);
                    else if (premode == 4)  // eco 3
                        c = OxyColor.FromArgb(50, 0, 0, 130);
                    else if (premode == 5)  // eco 4
                        c = OxyColor.FromArgb(50, 0, 0, 90);

                    chart.Rectangles.Add(new RectangleAnnotation()
                    {
                        Fill = c,
                        MinimumX = rect,
                        MaximumX = time,
                    });
                    rect = -1;
                }
                chart.Count = datanum;

                chart.selected = false;
                chart.searches = data;

                

                chart.Drawing();
                Vms.Add(chart);
            }));
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

        public async void ddd()
        {
            var client = head.getClient();
            string fluxquery = DB_influx.GetQuery_Search();
            var tables = await DB_influx.ExcuteInflux(client, fluxquery);
            string d, b, h, r;
            string dd = "", bb = "", hh = "", rr = "";
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                tables.ForEach(table =>
                {
                    
                    d = Convert.ToString(table.Records[0].Values["DANJI_ID"]);
                    b = Convert.ToString(table.Records[0].Values["BUILD_ID"]);
                    h = Convert.ToString(table.Records[0].Values["HOUSE_ID"]);
                    r = Convert.ToString(table.Records[0].Values["ROOM_ID"]);

                    if(dd  == d && bb == b&& hh == h && rr == r)
                    {
                        return;
                    }
                    dd = d;
                    bb = b;
                    hh = h;
                    rr = r;

                    Searches.Add(new SearchData()
                    {
                        DANJI_ID = Convert.ToString(table.Records[0].Values["DANJI_ID"]),
                        BUILD_ID = Convert.ToString(table.Records[0].Values["BUILD_ID"]),
                        HOUSE_ID = Convert.ToString(table.Records[0].Values["HOUSE_ID"]),
                        ROOM_ID = Convert.ToString(table.Records[0].Values["ROOM_ID"]),
                    });
                });
            }));
        }
        // 검색 조건 설정 윈도우 생성
        public async void RequestSelect(string opts = null)
        {
            double mintime;
            double maxtime;
            if (SOW == null)
            {
                (mintime, maxtime) = await Getminmax(opts);

                SOW = new SelectOptWindow();
                SOW.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                SOW.Topmost = true;
                SOW.OnChildTextInputEvent += new SelectOptWindow.OnChildTextInputHandler(SOW_OnChildTextInputEvent);
                SOW.Show();
            }
            else
            {
                (mintime, maxtime) = await Getminmax(opts);

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
        private void SOW_OnChildTextInputEvent(object Parameters)
        {
            if (Parameters != null)
            {
                if (SOW != null)
                {
                    SOW.Close();
                    SOW.OnChildTextInputEvent -= new SelectOptWindow.OnChildTextInputHandler(SOW_OnChildTextInputEvent);
                    SOW = null;
                }

                Thread t = new Thread(new ParameterizedThreadStart(CreateChart_influx));
                t.Start(Parameters);
                //CreateChart(options);
            }
            SOW = null;
        }
        // 입력 조건을 기반으로 SQL 서버로부터 TABLE 얻어오는 함수
        private DataTable GetDataTable(object obj)
        {
            var conn = head.getConnect();

            DataTable dt = DB_mysql.SelectTable(conn, DB_mysql.MakeQuery(obj));

            if (dt == null) MessageBox.Show("data table is null");

            return dt;

        }
        #endregion
        private async Task<(double,double)> Getminmax(string datas)
        {
            if (datas == null) return (-1, -1);
            var conn = head.getConnect();
            var client = head.getClient();

            string[] opts = datas.Split('\\');

            string danji = opts[0];
            string build = opts[1];
            string house = opts[2];
            string room = opts[3];
            double min = 0;
            double max = 0;

            string query = $"from(bucket:\"ZIPSAI/autogen\")" +
                $" |> range(start: 0)" +
                $" |> filter(fn: (r)=> r._measurement == \"{"sensor_data"}\")" +
                $" |> filter(fn: (r)=> r.DANJI_ID == \"{danji}\" and r.BUILD_ID == \"{build}\" and r.HOUSE_ID == \"{house}\" and r.ROOM_ID == \"{room}\")";


            var tables = await DB_influx.ExcuteInflux(client, query + " |> first()");
            if (tables.Count != 0)
            {
                Instant inst = (Instant)tables[0].Records[0].GetTime();
                min = inst.ToUnixTimeSeconds();
            }


            tables = await DB_influx.ExcuteInflux(client, query + " |> last()");
            if (tables.Count != 0)
            {
                Instant inst = (Instant)tables[0].Records[0].GetTime();
                max = inst.ToUnixTimeSeconds();
            }


            /*string query = $"SELECT * FROM {"sensor_data"} WHERE ID IS NOT NULL ";

            if (danji != "") danji = $" AND DANJI_ID = '{danji}'";
            if (build != "") build = $" AND BUILD_ID = '{build}'";
            if (house != "") house = $" AND HOUSE_ID = '{house}'";
            if (room != "") room = $" AND ROOM_ID = '{room}'";

            string minquery = query + danji + build + house + room + " LIMIT 1;";
            string maxquery = query + danji + build + house + room + " ORDER BY ID DESC LIMIT 1;";


            var table = DB_mysql.SelectTable(conn, minquery);
            if (table.Rows.Count != 0)
                min = Convert.ToDouble(table.Rows[0]["TIME"].ToString());
            

            table = DB_mysql.SelectTable(conn, maxquery);
            if(table.Rows.Count!=0)
                max = Convert.ToDouble(table.Rows[0]["TIME"].ToString()) + 1;
*/

            return (min, max);

        }

    }
}