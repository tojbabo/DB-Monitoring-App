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
using System.Windows.Shapes;

namespace MONITOR_APP.VIEW
{
    /// <summary>
    /// MainFrame.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainFrame : Window
    {
        BASE head;
        MV_MainFrame vm;
        public MainFrame()
        {

            InitializeComponent();
            head = BASE.getBASE();
            vm = head.getMV_MainFrame();
            this.DataContext = vm;
        }

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
            var conn = head.newConnect("52.79.127.111", "3306", "hansung_db", "hansung", "aidb4231@");
            
            if (conn == null) MessageBox.Show("Error in connect");
        }
    }
}
