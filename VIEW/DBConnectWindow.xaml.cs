using MONITOR_APP.MODEL;
using MONITOR_APP.VIEWMODEL;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MONITOR_APP.VIEW
{
    /// <summary>
    /// DBConnectWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DBConnectWindow : Window
    {

        BASE head;

        public DBConnectWindow()
        {
            InitializeComponent();

            head = BASE.getBASE();

            ip.Text = head.dbDetail.ip;
            port.Text = head.dbDetail.port;
            user.Text = head.dbDetail.uid;
            passwd.Password = head.dbDetail.passwd;
        }

        private void Buton_OK(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine($"{passwd.Password}");        
            try
            {
                head.dbDetail.ip = ip.Text;
                head.dbDetail.port = port.Text;
                head.dbDetail.uid = user.Text;
                head.dbDetail.passwd = passwd.Password;

                head.newClient();

                MessageBox.Show("연결 성공");
            }
            catch(Exception E)
            {
                MessageBox.Show("Connect err", E.Message);
                return;
            }

        }
        private void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void GridMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
