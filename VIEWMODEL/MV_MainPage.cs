using LiveCharts;
using LiveCharts.Wpf;
using MONITOR_APP.MODEL;
using MONITOR_APP.VIEW;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        void f()
        {
            var conn = head.getConnect();
            

        }


        public void CreateChart()
        {
            ChartValues<double> val = new ChartValues<double>();
            Random r = new Random();

            for (int i = 0; i < 100; ++i)
            {
                val.Add(r.NextDouble() * 100);
            }

            string[] a = { "a", "f", "m", "n", "g", "j", "t", "j", "b", "d", "c" };


            Datas.Add(new ChartData()
            {
                id = Datas.Count(),
                yData = new SeriesCollection()
                {
                    new LineSeries(){
                        Values = val,
                        Title = "TITLe",
                    }

                },
                Labels = a,
            });

        }

        public void ChartReset()
        {
            Datas.Clear();
        }
    }
}
