using MONITOR_APP.UTILITY;
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
    /// SelectOptWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SelectOptWindow : Window
    {
        public SelectOptWindow()
        {
            InitializeComponent();
        }

        public delegate void OnChildTextInputHandler(string Parameter);
        public event OnChildTextInputHandler OnChildTextInputEvent;

        private double min;
        private double max;

        #region Event

        private void Buton_OK(object sender, RoutedEventArgs e)
        {
            string table = TABLE.Text;
            string danji = ID_DANJI.Text;
            string build = ID_BUILD.Text;
            string house = ID_HOUSE.Text;
            string room = ID_ROOM.Text;

            long starttime = -1;
            long endtime = -1;
            if(startdap.SelectedDate != null)
                starttime = ((DateTimeOffset)startdap.SelectedDate).ToUnixTimeSeconds();

            if(enddatp.SelectedDate !=null)
                endtime = ((DateTimeOffset)enddatp.SelectedDate).ToUnixTimeSeconds();
            int interval = 3;

            if (RadioA.IsChecked == true) interval = 6;
            else if (RadioB.IsChecked == true) interval = 30;
            else if (RadioC.IsChecked == true) interval = 60;
            else if (RadioD.IsChecked == true) interval = 360;

            // table : 0
            // danji : 1
            // build : 2
            // house : 3
            // room : 4
            // cur_tmp : 5
            // set_tmp : 6
            // ONFF : 7
            // time_start : 8
            // time_end : 9
            // time_interval : 10
            // amount : 11

            string a = $"{table}\\{danji}\\{build}\\{house}\\{room}\\{CURTMP.IsChecked}\\{SETTMP.IsChecked}\\{ONFF.IsChecked}\\{starttime}\\{endtime}\\{interval}\\{amount.Text}";

            if (OnChildTextInputEvent != null) OnChildTextInputEvent(a);
        }
        private void Close(object sender, RoutedEventArgs e)
        {
            if (OnChildTextInputEvent != null) OnChildTextInputEvent(null);
            this.Close();
        }
        private void GridMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        public void SetData(string datas)
        {
            string[] opts = datas.Split('\\');
            if (opts.Length > 2)
            {
                ID_DANJI.Text = opts[2];
                ID_BUILD.Text = opts[3];
                ID_HOUSE.Text = opts[4];
                ID_ROOM.Text = opts[5];
            }

            min = Convert.ToDouble(opts[0]);
            max = Convert.ToDouble(opts[1]);
            if (min != -1)
                startdap.SelectedDate = TimeConverter.ConvertTimestamp(min);
            if (max != -1)
                enddatp.SelectedDate = TimeConverter.ConvertTimestamp(max);
        }
        #endregion

    }
}
