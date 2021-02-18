using MONITOR_APP.UTILITY;
using MONITOR_APP.VIEWMODEL;
using MySql.Data.MySqlClient;
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

        private void Button_DBConn(object sender, RoutedEventArgs e)
        {
            // DBConnectWindow DBC = new DBConnectWindow(head);
            //DBC.Show();
            DB_influx.f();
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
        #endregion
    }
}
