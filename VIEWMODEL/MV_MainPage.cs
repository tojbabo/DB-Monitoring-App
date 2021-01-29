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
        public ObservableCollection<ChartData> Datas { get; }
        BASE head;
        

        public MV_MainPage()
        {
            head = BASE.getBASE();
            Datas = new ObservableCollection<ChartData>();
        }

        DataTable f(string table,string opt)
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

            DataTable dt = f(table,danji);
            ChartValues<double> ydata = new ChartValues<double>();
            List<string> xdata = new List<string>();

            cs = new ChartValues<CustomVm>();

            foreach (DataRow r in dt.Rows)
            {
                try
                {
                    string temp = r["SET_TEMP"].ToString();
                    double d = Convert.ToDouble(temp);
                    ydata.Add(d);
                    xdata.Add(r["TIME"].ToString());
                }
                catch { continue; }

                cs.Add(new CustomVm
                {
                    Name = r["SET_TEMP"].ToString(),
                    LastName = r["TIME"].ToString()
                }) ;

            }

            /*var cutomVmMapper = Mappers.Xy<CustomVm>()
                .X((value, index) => index)
                .Y(value => index);*/

            /*Datas.Add(new ChartData()
            {
                id = Datas.Count(),
                yData = new SeriesCollection()
                {
                    new LineSeries(){
                        Values = ydata,
                        Title = $"{table} - {danji}",
                    }

                },
                Labels = xdata.ToArray(),
            });*/

            
            

        }

        public ChartValues<CustomVm> cs { get; set; }


        public void ChartReset()
        {
            Datas.Clear();
        }

        
    }
}