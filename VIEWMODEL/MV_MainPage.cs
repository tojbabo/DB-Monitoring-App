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

        DataTable GetDataTable(string table,string opt)
        {
            var conn = head.getConnect();

            DataTable dt = MySQL.SelectTable(conn, table, opt);

            if (dt == null) MessageBox.Show("data table is null");

            return dt;

        }


        SelectOptWindow SOW;

        public void CreateChart()
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

            if (SOW != null)
            {
                SOW.Close();
                SOW.OnChildTextInputEvent -= new SelectOptWindow.OnChildTextInputHandler(SOW_OnChildTextInputEvent);
                SOW = null;
            }

            CreateGraph(options);
        }

        void CreateGraph(string[] options)
        {
            string table = options[0];
            string danji = options[1];

            DataTable dt = GetDataTable(table,danji);

            ChartData chart = new ChartData();

            foreach(DataRow r in dt.Rows)
            {
                chart.data.Add(Convert.ToDouble(r["SET_TEMP"].ToString()));
                chart.Labels.Add(r["TIME"].ToString());
            }
            
            Vms.Add(chart);


        }

        public void ChartReset()
        {
            Vms.Clear();
        }

        
    }
}