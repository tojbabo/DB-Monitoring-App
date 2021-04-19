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
    /// StaticSetWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class StaticSetWindow : Window
    {


        public delegate void OnChildTextInputHandler(object Parameter);
        public event OnChildTextInputHandler OnChildTextInputEvent;

        BASE head;
        public StaticSetWindow()
        {
            InitializeComponent();
            head = BASE.getBASE();

            maxY.Text = $"{head.val.AXISYMAX}";
            minY.Text = $"{head.val.AXISYMIN}";
            valOn.Text = $"{head.val.ONVALUE}";
            valOff.Text = $"{head.val.OFFVALUE}";

        }

        private void Buton_OK(object sender, RoutedEventArgs e)
        {
            head.val.AXISYMAX = Convert.ToInt32(maxY.Text);
            head.val.AXISYMIN = Convert.ToInt32(minY.Text);
            head.val.ONVALUE = Convert.ToInt32(valOn.Text);
            head.val.OFFVALUE = Convert.ToInt32(valOff.Text);

            OnChildTextInputEvent(null);
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
