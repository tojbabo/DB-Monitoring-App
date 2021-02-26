
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
        MainPage mp;

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
        // 생성된 모든 CHART를 지우는 함수
        public void ChartReset()
        {
            Vms.Clear();
        }
        // CHART 생성 함수
        private async void CreateChart_influx(object opt)
        {
            SearchData data = (SearchData)opt;
            InfluxDBClient client = head.getClient();

            var tables = await DB_influx.ExcuteInflux(client, DB_influx.GetQuery(data));

            if (tables.Count == 0)
            {
                MessageBox.Show("데이터를 찾을 수 없습니다.");
                return;
            }

            var chart = ChartDataInput(tables);
            chart.searches = data;

            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                chart.Drawing();
                Vms.Add(chart);
            }));
        }
        // CHART 여러개 동시 생성
        private void CreateCharts_influx(object opt)
        {
            List<SearchData> datas = (List<SearchData>)opt;

            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                mp.Tiling(e_bool.False);
            }));

            foreach (var data in datas)
            {
                CreateChart_influx(data);
            }
        }
        ChartData ChartDataInput(List<FluxTable> tables)
        {
            ChartData chart = new ChartData();
            double set, cur, onoff, time;
            int aimode, premode = -1;
            int datanum = 0;

            double rect = -1;
            time = 0;
            tables.ForEach(table =>
            {
                table.Records.ForEach(cell =>
                {
                    Instant inst = (Instant)cell.GetTime();
                    time = DateTimeAxis.ToDouble(TimeConverter.ConvertTimestamp(inst.ToUnixTimeSeconds()));

                    string field = cell.GetField();

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
                    else if (field == "AI_MODE")
                    {
                        aimode = Convert.ToInt32(cell.GetValue());
                        //Console.Write($" {aimode}");

                        if (premode == -1) // 초기 설정일 때
                        {
                            premode = aimode;
                            if (premode != 0)
                                rect = time;
                        }
                        else if (premode != aimode) // 중간에 모드가 바뀔 때
                        {
                            if (rect == -1)
                                rect = time;

                            else if (rect != -1)
                            {
                                if (premode != 0)
                                {
                                    chart.Rectangles.Add(new RectangleAnnotation()
                                    {
                                        Fill = GetColor(premode),
                                        MinimumX = rect,
                                        MaximumX = time,
                                    });
                                    rect = -1;
                                }
                            }

                            premode = aimode;
                        }
                    }
                });
            });
            if (rect != -1)
            {
                chart.Rectangles.Add(new RectangleAnnotation()
                {
                    Fill = GetColor(premode),
                    MinimumX = rect,
                    MaximumX = time,
                });
                rect = -1;
            }

            chart.Count = datanum;
            chart.selected = false;

            return chart;
        }
        public void Chart_Modify()
        {
            Console.WriteLine("event ok");
        }
        #endregion
        
        // SEARCHDATA 생성
        public async void CreateSearchData_Influx()
        {
            var client = head.getClient();
            string fluxquery = DB_influx.GetQuery_Search();
            var tables = await DB_influx.ExcuteInflux(client, fluxquery);
            string danji, build, house, room;
            string _danji = "", _build = "", _house = "", _room = "";
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                tables.ForEach(table =>
                {

                    danji = Convert.ToString(table.Records[0].Values["DANJI_ID"]);
                    build = Convert.ToString(table.Records[0].Values["BUILD_ID"]);
                    house = Convert.ToString(table.Records[0].Values["HOUSE_ID"]);
                    room = Convert.ToString(table.Records[0].Values["ROOM_ID"]);

                    if (_danji == danji && _build == build && _house == house && _room == room)
                    {
                        return;
                    }
                    _danji = danji;
                    _build = build;
                    _house = house;
                    _room = room;

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
        // SEARCH WINDOW 생성
        public async void RequestSelect(SearchData s = null, object o = null)
        {

            if (SOW == null)
            {
                s = await Getminmax(s);

                SOW = new SelectOptWindow();
                SOW.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                SOW.Topmost = true;
                SOW.OnChildTextInputEvent += new SelectOptWindow.OnChildTextInputHandler(SOW_OnChildTextInputEvent);
                SOW.Show();
            }
            else
            {
                s = await Getminmax(s);

                SOW.Focus();
            }

            if (o != null) mp = (MainPage)o;

            SOW.SetData(s);
        }
        // SEARCH WINDOW 생성, <복수개 선택>
        public void RequestSelect(List<SearchData> searchDatas, object o = null)
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

            if (o != null) mp = (MainPage)o;

            SOW.SetData(searchDatas);
        }
        // SEARCH WINDOW 응답 함수
        private void SOW_OnChildTextInputEvent(object Parameters)
        {
            if (Parameters != null)
            {
                if (mp != null)
                {
                    mp.Grid_search.Visibility = Visibility.Collapsed;
                }
                if (SOW != null)
                {
                    SOW.Close();
                    SOW.OnChildTextInputEvent -= new SelectOptWindow.OnChildTextInputHandler(SOW_OnChildTextInputEvent);
                    SOW = null;
                }
                if (Parameters is SearchData)
                {
                    Thread t = new Thread(new ParameterizedThreadStart(CreateChart_influx));
                    t.Start(Parameters);
                }
                else if (Parameters is List<SearchData>)
                {
                    Thread t = new Thread(new ParameterizedThreadStart(CreateCharts_influx));
                    t.Start(Parameters);
                }
                //CreateChart(options);
            }
            SOW = null;
        }
        // 최소 , 최대 시간 구하는 함수
        private async Task<SearchData> Getminmax(SearchData s)
        {
            if (s == null) return new SearchData();
            var conn = head.getConnect();
            var client = head.getClient();

            double min = 0;
            double max = 0;

            string query = DB_influx.GetQuery_zero(s);

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

            s.mintime = min;
            s.maxtime = max;

            #region mysql
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
            #endregion 

            return s;

        }
        private OxyColor GetColor(int mode)
        {
            if (mode == 1)  // ai mode
                return OxyColor.FromArgb(50, 0, 210, 0);
            else if (mode == 2)  // eco 1
                return OxyColor.FromArgb(50, 0, 0, 210);
            else if (mode == 3)  // eco 2
                return OxyColor.FromArgb(50, 0, 0, 170);
            else if (mode == 4)  // eco 3
                return OxyColor.FromArgb(50, 0, 0, 130);
            else if (mode == 5)  // eco 4
                return OxyColor.FromArgb(50, 0, 0, 90);

            return OxyColors.Undefined;
        }

    }
}