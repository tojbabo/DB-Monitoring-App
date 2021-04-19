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
            vm = head.getMV_MainFrame();
            this.DataContext = vm;
            
            mp = new MainPage();
            Page.NavigationService.Navigate(mp);

            //this.ShowInTaskbar = false;
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
            vm.DBConnectSetting();
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

        private void Button_Menu(object sender, RoutedEventArgs e)
        {
            Grid_Opt.Visibility = Grid_Opt.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }

        private void Button_Save(object sender, RoutedEventArgs e)
        {
            head.getMV_MainPage().save();
            
        }

        private void Button_GraphSet(object sender, RoutedEventArgs e)
        {
            vm.StaticValueSetting();
        }
    }
}
