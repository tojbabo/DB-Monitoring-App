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
        private e_bool searchmode;
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
            if (searchmode == e_bool.Single)
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

                _searchData.amount = Convert.ToDouble(amount.Text);


                if (OnChildTextInputEvent != null) OnChildTextInputEvent(_searchData);
            }
            else if(searchmode == e_bool.Multi)
            {
                foreach(var v in _searchDatas)
                {
                    v.TABLE = TABLE.Text;

                    if (startdap.SelectedDate != null)
                        v.mintime = ((DateTimeOffset)startdap.SelectedDate).ToUnixTimeSeconds();

                    if (enddatp.SelectedDate != null)
                        v.maxtime = ((DateTimeOffset)enddatp.SelectedDate).ToUnixTimeSeconds();

                    v.amount = Convert.ToDouble(amount.Text);

                }
                if (OnChildTextInputEvent != null) OnChildTextInputEvent(_searchDatas);
            }
            else if(searchmode == e_bool.Reuse)
            {
                string op = $"{((DateTimeOffset)startdap.SelectedDate).ToUnixTimeSeconds()}\\{((DateTimeOffset)enddatp.SelectedDate).ToUnixTimeSeconds()}";
                if (OnChildTextInputEvent != null) OnChildTextInputEvent(op);
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
            searchmode = e_bool.Single;

            ID_DANJI.Text = s.DANJI_ID;
            ID_BUILD.Text = s.BUILD_ID;
            ID_HOUSE.Text = s.HOUSE_ID;
            ID_ROOM.Text = s.ROOM_ID;
            ID_DANJI.IsEnabled = true;
            ID_BUILD.IsEnabled = true;
            ID_HOUSE.IsEnabled = true;
            ID_ROOM.IsEnabled = true;

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
            searchmode = e_bool.Multi;

            ID_DANJI.IsEnabled = false;
            ID_BUILD.IsEnabled = false;
            ID_HOUSE.IsEnabled = false;
            ID_ROOM.IsEnabled = false;
            
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
        public void SetData()
        {
            searchmode = e_bool.Reuse;
            ID_DANJI.Text = " - ";
            ID_DANJI.IsEnabled = false;
            ID_BUILD.Text = " - ";
            ID_BUILD.IsEnabled = false;
            ID_HOUSE.Text = " - ";
            ID_HOUSE.IsEnabled = false;
            ID_ROOM.Text = " - ";
            ID_ROOM.IsEnabled = false;

        }
        #endregion

    }
}
