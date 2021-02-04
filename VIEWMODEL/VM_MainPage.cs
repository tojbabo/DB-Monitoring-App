using MONITOR_APP.MODEL;
using MONITOR_APP.UTILITY;
using MONITOR_APP.VIEW;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;
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


            ChartData chart = new ChartData();
            Vms.Add(chart);
            Random r = new Random();
            for (int i = 0; i < 100; i++)
            {
                chart.title = "test data";
                chart.data.Add(r.Next(0, 10));
                chart.selected = false;
            }
            return;
        }

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
        DataTable GetDataTable(string[] opts)
        {
            var conn = head.getConnect();

            DataTable dt = MySQL.SelectTable(conn,MySQL.MakeQuery(opts));

            if (dt == null) MessageBox.Show("data table is null");

            return dt;

        }
        void CreateChart(object opt)
        {
            string[] options = (string[])opt;
            string table = options[0];
            string danji = options[1];
            string build = options[2];
            string house = options[3];
            string room = options[4];

            ChartData chart = new ChartData();

            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                Vms.Add(chart);
                if (danji == "-1")
                {
                    Random r = new Random();
                    for (int i = 0; i < 100; i++)
                    {
                        chart.title = "test data";
                        chart.data.Add(r.Next(0, 10));
                        chart.selected = false;
                    }
                    return;
                }
            }));


            DataTable dt = GetDataTable(options);


            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                foreach (DataRow r in dt.Rows)
                {
                    chart.data.Add(Convert.ToDouble(r["SET_TEMP"].ToString()));
                    chart.Labels.Add(TimeConverter.GetTime(r["TIME"].ToString()));
                    chart.title = $"{danji} - {build} - {house} - {room}";
                }
            }));
        }
        public void ChartReset()
        {
            Vms.Clear();
        }
        public void GetDetailChart(ChartData d)
        {
            Console.WriteLine($"{d.title}");

            Console.WriteLine($"{d.data.Count()}");

        }


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
        void SOW_OnChildTextInputEvent(string Parameters)
        {
            if (Parameters != null)
            {
                string[] options = Parameters.Split('\\');

                string d = "asd\\asdfasd\\vdfdfdf\\\\\\\\asdfasdf";
                string[] df = d.Split('\\');

                if (SOW != null)
                {
                    SOW.Close();
                    SOW.OnChildTextInputEvent -= new SelectOptWindow.OnChildTextInputHandler(SOW_OnChildTextInputEvent);
                    SOW = null;
                }

                Thread t = new Thread(new ParameterizedThreadStart(CreateChart));
                t.Start(options);
            }
            SOW = null;
        }
    }
}