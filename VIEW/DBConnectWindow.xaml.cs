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

        public DBConnectWindow(BASE head)
        {
            InitializeComponent();

            this.head = head;

            ip.Text = "52.79.127.111";
            port.Text = "3306";
            db.Text = "hansung_db";
            user.Text = "hansung";
            passwd.Password = "aidb4231@";
        }

        private void Buton_OK(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine($"{passwd.Password}");        
            try
            {
                head.newConnect(ip.Text, port.Text, db.Text, user.Text, passwd.Password);

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
