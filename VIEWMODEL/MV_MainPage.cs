using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using MONITOR_APP.MODEL;
using MONITOR_APP.UTILITY;
using MONITOR_APP.VIEW;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MONITOR_APP.VIEWMODEL
{

    public class MV_MainPage
    {
        BASE head;
        public ObservableCollection<ChartData> Vms { get; set; }
        //public ChartValues<CustomVm> cs { get; set; }


        public MV_MainPage()
        {
            head = BASE.getBASE();
            Vms = new ObservableCollection<ChartData>();
        }

        DataTable GetDataTable(string[] opts)
        {
            var conn = head.getConnect();

            DataTable dt = MySQL.SelectTable(conn,MySQL.MakeQuery(opts));

            if (dt == null) MessageBox.Show("data table is null");

            return dt;

        }


        SelectOptWindow SOW;

        public void RequestSelect()
        {
            SOW = new SelectOptWindow();

            SOW.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            SOW.Topmost = true;
            SOW.OnChildTextInputEvent += new SelectOptWindow.OnChildTextInputHandler(SOW_OnChildTextInputEvent);
            SOW.ShowDialog();
        }

        void SOW_OnChildTextInputEvent(string Parameters)
        {
            string[] options = Parameters.Split('\\');

            string d = "asd\\asdfasd\\vdfdfdf\\\\\\\\asdfasdf";
            string[] df = d.Split('\\');
            Console.WriteLine($"data is : {df.Count()}");

            if (SOW != null)
            {
                SOW.Close();
                SOW.OnChildTextInputEvent -= new SelectOptWindow.OnChildTextInputHandler(SOW_OnChildTextInputEvent);
                SOW = null;
            }

            CreateChart(options);
        }

        void CreateChart(string[] options)
        {
            string table = options[0];
            string danji = options[1];

            DataTable dt = GetDataTable(options);

            ChartData chart = new ChartData();

            foreach (DataRow r in dt.Rows)
            {
                chart.data.Add(Convert.ToDouble(r["SET_TEMP"].ToString()));
                chart.Labels.Add(Timer.GetTime(r["TIME"].ToString()));
                chart.title = $"{table}/{danji}";
            }
            Vms.Add(chart);
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
        //SELECT DISTINCT DANJI_ID, BUILD_ID, HOUSE_ID, ROOM_ID FROM sensor_data;
        // 전체 찾는 SQL
    }
}