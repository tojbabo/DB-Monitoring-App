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
        private bool multicheck = false;
        private SearchData _searchData;
        private List<SearchData> _searchDatas;
        public SelectOptWindow()
        {
            InitializeComponent();
        }

        public delegate void OnChildTextInputHandler(object Parameter);
        public event OnChildTextInputHandler OnChildTextInputEvent;

        private double min;
        private double max;

        public DateTime? DataPicker { get; private set; }

        #region Event

        private void Buton_OK(object sender, RoutedEventArgs e)
        {
            if (multicheck == false)
            {
                _searchData.TABLE = TABLE.Text;
                _searchData.DANJI_ID = ID_DANJI.Text;
                _searchData.BUILD_ID = ID_BUILD.Text;
                _searchData.HOUSE_ID = ID_HOUSE.Text;
                _searchData.ROOM_ID = ID_ROOM.Text;


                if (startdap.SelectedDate != null)
                    _searchData.mintime = ((DateTimeOffset)startdap.SelectedDate).ToUnixTimeSeconds();

                if (enddatp.SelectedDate != null)
                    _searchData.maxtime = ((DateTimeOffset)enddatp.SelectedDate).ToUnixTimeSeconds();

                if (RadioA.IsChecked == true) _searchData.interval = 0;
                else if (RadioB.IsChecked == true) _searchData.interval = 1;
                else if (RadioC.IsChecked == true) _searchData.interval = 2;
                else if (RadioD.IsChecked == true) _searchData.interval = 3;

                _searchData.amount = Convert.ToDouble(amount.Text);

                _searchData.tmp_set = (bool)SETTMP.IsChecked;
                _searchData.tmp_cur = (bool)CURTMP.IsChecked;
                _searchData.on_off = (bool)ONFF.IsChecked;

                if (OnChildTextInputEvent != null) OnChildTextInputEvent(_searchData);
            }
            else
            {
                foreach(var v in _searchDatas)
                {
                    v.TABLE = TABLE.Text;

                    if (startdap.SelectedDate != null)
                        v.mintime = ((DateTimeOffset)startdap.SelectedDate).ToUnixTimeSeconds();

                    if (enddatp.SelectedDate != null)
                        v.maxtime = ((DateTimeOffset)enddatp.SelectedDate).ToUnixTimeSeconds();

                    if (RadioA.IsChecked == true) v.interval = 0;
                    else if (RadioB.IsChecked == true) v.interval = 1;
                    else if (RadioC.IsChecked == true) v.interval = 2;
                    else if (RadioD.IsChecked == true) v.interval = 3;

                    v.amount = Convert.ToDouble(amount.Text);

                    v.tmp_set = (bool)SETTMP.IsChecked;
                    v.tmp_cur = (bool)CURTMP.IsChecked;
                    v.on_off = (bool)ONFF.IsChecked;
                }
                if (OnChildTextInputEvent != null) OnChildTextInputEvent(_searchDatas);
            }
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
        public void SetData(SearchData s)
        {
            multicheck = false;
            ID_DANJI.Text = s.DANJI_ID;
            ID_BUILD.Text = s.BUILD_ID;
            ID_HOUSE.Text = s.HOUSE_ID;
            ID_ROOM.Text = s.ROOM_ID;

            min = Convert.ToDouble(s.mintime);
            max = Convert.ToDouble(s.maxtime);
            if (min != -1)
                startdap.SelectedDate = TimeConverter.ConvertTimestamp(min);
            if (max != -1)
                enddatp.SelectedDate = TimeConverter.ConvertTimestamp(max);

            _searchData = s;
        }
        public void SetData(List<SearchData> searchDatas)
        {
            multicheck = true;
            SearchData first = searchDatas[0];
            SearchData last = searchDatas[searchDatas.Count - 1];

            ID_DANJI.Text = $"{first.DANJI_ID} - {last.DANJI_ID}";
            ID_BUILD.Text = $"{first.BUILD_ID} - {last.BUILD_ID}";
            ID_HOUSE.Text = $"{first.HOUSE_ID} - {last.HOUSE_ID}";
            ID_ROOM.Text = $"{first.ROOM_ID} - {last.ROOM_ID}";

            startdap.SelectedDate = DateTime.Now.AddDays(-1);
            enddatp.SelectedDate = DateTime.Now;

            _searchDatas = searchDatas;
        }
        #endregion

    }
}
