using InfluxDB.Client;
using InfluxDB.Client.Core.Flux.Domain;
using MONITOR_APP.UTILITY;
using MONITOR_APP.VIEWMODEL;
using MySql.Data.MySqlClient;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MONITOR_APP.VIEW
{
    /// <summary>
    /// MainFrame.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainFrame : Window
    {
        BASE head;
        VM_MainFrame vm;
        MainPage mp;
        public MainFrame()
        {
            InitializeComponent();

            head = BASE.getBASE();
            head.newConnect("52.79.127.111", "3306", "hansung_db", "hansung", "aidb4231@");
            head.newClient("52.79.127.111", "8086", "hsai", "han401#");
            vm = head.getMV_MainFrame();
            this.DataContext = vm;
            
            mp = new MainPage();
            Page.NavigationService.Navigate(mp);
        }

        #region Event

        private void Button_EXIT(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void GridMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private async void Button_DBConn(object sender, RoutedEventArgs e)
        {
            // DBConnectWindow DBC = new DBConnectWindow(head);
            //DBC.Show();
            //DB_influx.f();

            /*InfluxDBClient client = DB_influx.GetClient();

            var query = $"from(bucket:\"ZIPSAI/autogen\")" +
                $" |> range(start: 0)" +
                $" |> filter(fn: (r)=> r._measurement == \"{"sensor_data"}\")" +
                $" |> filter(fn: (r)=> r.DANJI_ID == \"2323\" and r.BUILD_ID == \"202\" and r.HOUSE_ID == \"101\" and r.ROOM_ID == \"0\")" +
                $" |> first()";


            var tables = await DB_influx.ExcuteInflux(client, query);
            Console.WriteLine($"good");*/

            /*tables.ForEach(record =>
            {
                var cell = record.Records[0];

                Console.WriteLine($"[{record.Records[0].Values["DANJI_ID"]}/{record.Records[0].Values["BUILD_ID"]}" +
                    $"/{record.Records[0].Values["HOUSE_ID"]}/{record.Records[0].Values["ROOM_ID"]}] .. {record.Records[0].Values["SN"]}");

                Console.WriteLine($"detail is : {cell.GetTime()} {cell.GetMeasurement()}: {cell.GetField()} {cell.GetValue()}");
            });*/
            //head.newConnect("52.79.127.111", "3306", "hansung_db", "hansung", "aidb4231@");
        }


        private void Button_Maximize(object sender, RoutedEventArgs e)
        {
            this.WindowState = (this.WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
        }

        private void Button_Minimize(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
            {
                this.WindowState = (this.WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
            }

        }
        private void Button_Main(object sender, RoutedEventArgs e)
        {
            mp.GridTurnOnOff();
        }
        private void Button_Tiling(object sender, RoutedEventArgs e)
        {
            mp.Tiling();
        }
        #endregion

    }
}
