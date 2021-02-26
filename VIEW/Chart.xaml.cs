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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MONITOR_APP.VIEW
{
    /// <summary>
    /// Chart.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Chart : UserControl
    {
        public Chart()
        {
            InitializeComponent();
        }
        public event MouseButtonEventHandler Graph_RightDown;
        private void _Graph_RightDown(object sender, MouseButtonEventArgs e)
        {
            if (Graph_RightDown != null)
            {
                Graph_RightDown(this, e);
            }

        }

        public event MouseButtonEventHandler Graph_DoubleClick;
        private void _Graph_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Graph_DoubleClick != null)
            {
                Graph_DoubleClick(this, e);
            }
        }

        public event MouseButtonEventHandler Graph_LeftDown;
        private void _Graph_LeftDown(object sender, MouseButtonEventArgs e)
        {
            if (Graph_LeftDown != null)
            {
                Graph_LeftDown(this, e);
            }

        }

        public event RoutedEventHandler Button_Reload;
        private void _Button_Reload(object sender, RoutedEventArgs e)
        {
            if (Button_Reload != null)
            {
                Button_Reload(this, e);
            }
        }

        public event RoutedEventHandler Check_Graph;
        private void _Check_Graph(object sender, RoutedEventArgs e)
        {
            if (Check_Graph != null)
            {
                Check_Graph(this, e);
            }
        }
    }
}
