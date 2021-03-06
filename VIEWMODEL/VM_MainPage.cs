﻿
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
        public void ChartReset(ChartData chart = null)
        {
            if (chart == null)
                Vms.Clear();
            else
                Vms.Remove(chart);
        }
        // CHART 생성 함수
        private async void CreateChart_influx(object Searchdata)
        {
            SearchData data = (SearchData)Searchdata;
            var chart = new ChartData(head.val); 

            InfluxDBClient client = head.getClient();
            //var tables = await DB_influx.ExcuteInflux(client, DB_influx.GetQuery_Group(data));
            var tables = await DB_influx.ExcuteInflux(client, DB_influx.GetQuery(data));

            if (tables.Count == 0)
            {
                MessageBox.Show($"[ROOM:{data.ROOM_ID}] 데이터를 찾을 수 없습니다.");
                return;
            }


            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                Vms.Add(chart);

                ChartDataInput(ref chart,tables);

                chart.searches = new SearchData(data);
                chart.Drawing();
            }));
        }
        // CHART 여러개 동시 생성
        private void CreateCharts_influx(object SearchList)
        {
            List<SearchData> datas = (List<SearchData>)SearchList;

            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                mp.Tiling(e_bool.False);
            }));

            foreach (var data in datas)
            {
                CreateChart_influx(data);
            }
        }

        private async void ReloadChart_influx(object Chartdata)
        {
            ChartData chart = (ChartData)Chartdata;

            chart.set.Clear();
            chart.cur.Clear();
            chart.onff.Clear();
            chart.Rectangles.Clear();

            chart.onfftime = 0;

            InfluxDBClient client = head.getClient();

            var tables = await DB_influx.ExcuteInflux(client, DB_influx.GetQuery(chart.searches));

            if (tables.Count == 0)
            {
                MessageBox.Show("데이터를 찾을 수 없습니다.");
                return;
            }

            ChartDataInput(ref chart, tables);

            int index = Vms.IndexOf(chart);
            Vms.Remove(chart);
            chart.ReFresh();
            Vms.Insert(index, chart);
        }
        void ChartDataInput(ref ChartData chart,List<FluxTable> tables)
        {
            double set, cur, onoff, time;
            int aimode, premode = -1;
            double rect = -1;
            time = 0;
            int count = 0;
            double checker = -1;
            double unixtime = 0;

            foreach (var table in tables)
            {
                foreach (var cell in table.Records)
                {
                    //Console.WriteLine($"[{cell.GetTime()}] {cell.GetField()}:{cell.GetValue()}");
                    Instant inst = (Instant)cell.GetTime();
                    unixtime = inst.ToUnixTimeSeconds();
                    time = DateTimeAxis.ToDouble(TimeConverter.ConvertTimestamp(unixtime - 32400)) ;

                    string field = cell.GetField();

                    if (field == "SET_TEMP")
                    {
                        //Console.WriteLine($"[{cell.GetTime()}] {cell.GetField()}:{cell.GetValue()}");
                        set = DB_influx.GetData(cell, "SET_TEMP");
                        if (!double.IsNaN(set))
                        {
                            chart.set.Add(new DataPoint(time, set / 10));
                            continue;
                        }
                    }
                    else if (field == "CUR_TEMP")
                    {
                        cur = DB_influx.GetData(cell, "CUR_TEMP");
                        if (!double.IsNaN(cur))
                        {
                            chart.cur.Add(new DataPoint(time, cur / 10));
                            continue;
                        }
                    }
                    else if (field == "VALVE_STATUS")
                    {
                        onoff = DB_influx.GetData(cell, "VALVE_STATUS");
                        if (!double.IsNaN(onoff))
                        {
                            chart.onff.Add(new DataPoint(time, (onoff == 1) ? chart.onvalue : chart.offvalue));
                            if (onoff == 1)
                            {
                                count++; 
                                if (checker == -1)
                                {
                                    checker = unixtime;
                                }
                            }
                            else if (onoff == 0)
                            {
                                if (checker != -1)
                                {
                                    chart.onfftime += unixtime - checker;
                                    checker = -1;
                                }
                            }   
                            continue;
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
                }; 
            };

            // 그리던 사각형이 끝까지 가는 경우
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

            if(checker != -1)
            {
                chart.onfftime += unixtime - checker;
            }

            chart.onffcount = count;
            chart.selected = false;

        }
        public void Chart_Modify()
        {
            if (Vms.Count != 0)
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

                SOW.SetData();
            }
        }
        public void Chart_Reload(ChartData d)
        {
            ReloadChart_influx(d);
        }
        public void Chart_Reload(object time)
        {
            var times = Convert.ToString(time).Split('\\');
            foreach (var v in Vms)
            {
                v.searches.mintime = Convert.ToDouble(times[0]);
                v.searches.maxtime = Convert.ToDouble(times[1]);

                Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    ReloadChart_influx(v);
                }));
            }
        }
        public void Chart_Reload()
        {
            foreach(ChartData d in Vms)
            {
                d.ymax = head.val.AXISYMAX;
                d.ymin = head.val.AXISYMIN;
                d.onvalue = head.val.ONVALUE;
                d.offvalue = head.val.OFFVALUE;

                ReloadChart_influx(d);
            }
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
                else if (Parameters is string)
                {
                    Thread t = new Thread(new ParameterizedThreadStart(Chart_Reload));
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

        public void save()
        {
            foreach (var v in Vms)
            {
                CSV_Stream.CSV_Write(v);
            }
        }

    }
}