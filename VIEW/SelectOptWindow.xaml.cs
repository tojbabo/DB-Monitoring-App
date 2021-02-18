using MONITOR_APP.MODEL;
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

        public delegate void OnChildTextInputHandler(object Parameter);
        public event OnChildTextInputHandler OnChildTextInputEvent;

        private double min;
        private double max;

        #region Event

        private void Buton_OK(object sender, RoutedEventArgs e)
        {
            SearchData SEARCH = new SearchData();
            
            SEARCH.TABLE = TABLE.Text;
            SEARCH.DANJI_ID = ID_DANJI.Text;
            SEARCH.BUILD_ID = ID_BUILD.Text;
            SEARCH.HOUSE_ID = ID_HOUSE.Text;
            SEARCH.ROOM_ID = ID_ROOM.Text;


            if(startdap.SelectedDate != null)
                SEARCH.mintime = ((DateTimeOffset)startdap.SelectedDate).ToUnixTimeSeconds();

            if(enddatp.SelectedDate !=null)
                SEARCH.maxtime = ((DateTimeOffset)enddatp.SelectedDate).ToUnixTimeSeconds();

            if (RadioA.IsChecked == true) SEARCH.interval = 0;
            else if (RadioB.IsChecked == true) SEARCH.interval = 1;
            else if (RadioC.IsChecked == true) SEARCH.interval = 2;
            else if (RadioD.IsChecked == true) SEARCH.interval = 3;

            SEARCH.amount = Convert.ToDouble(amount.Text);

            SEARCH.tmp_set = (bool)SETTMP.IsChecked;
            SEARCH.tmp_cur = (bool)CURTMP.IsChecked;
            SEARCH.on_off = (bool)ONFF.IsChecked;

            if (OnChildTextInputEvent != null) OnChildTextInputEvent(SEARCH);
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
