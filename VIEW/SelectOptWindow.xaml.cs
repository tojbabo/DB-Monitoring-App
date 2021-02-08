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

        List<ComboBoxItem> monthlist;
        List<ComboBoxItem> daylist;
        List<ComboBoxItem> hourlist;
        List<ComboBoxItem> minutelist;
        public SelectOptWindow(string time)
        {
            InitializeComponent();

            string[] opts = time.Split('\\');
            double min = Convert.ToDouble(opts[0]);
            double max = Convert.ToDouble(opts[1]);


            monthlist = new List<ComboBoxItem>();
            daylist = new List<ComboBoxItem>();
            hourlist = new List<ComboBoxItem>();
            minutelist = new List<ComboBoxItem>();

            for (int i =0; i < 60; i++)
            {
                if (i < 12)
                    monthlist.Add(new ComboBoxItem()
                    {
                        Content = $"{i + 1}",

                    });
                if(i <24)
                    hourlist.Add(new ComboBoxItem()
                    {
                        Content = $"{i + 1}",

                    });
                if(i<30)
                    daylist.Add(new ComboBoxItem()
                    {
                        Content = $"{i + 1}",

                    });

                minutelist.Add(new ComboBoxItem()
                {
                    Content = $"{i + 1}",

                });
            }

            month.ItemsSource = monthlist;
            monthto.ItemsSource = monthlist;
            day.ItemsSource = daylist;
            dayto.ItemsSource = daylist;
            hour.ItemsSource = hourlist;
            hourto.ItemsSource = hourlist;
            minute.ItemsSource = minutelist;
            minuteto.ItemsSource = minutelist;

            var t1 = TimeConverter.ConvertTimestamp(min);
            month.SelectedIndex = t1.Month-1;
            day.SelectedIndex = t1.Day-1;
            hour.SelectedIndex = t1.Hour-1;
            minute.SelectedIndex = t1.Minute-1;


            t1 = TimeConverter.ConvertTimestamp(max);
            monthto.SelectedIndex = t1.Month - 1;
            dayto.SelectedIndex = t1.Day - 1;
            hourto.SelectedIndex = t1.Hour - 1;
            minuteto.SelectedIndex = t1.Minute - 1;

        }

        public delegate void OnChildTextInputHandler(string Parameter);
        public event OnChildTextInputHandler OnChildTextInputEvent;


        #region Event

        private void Buton_OK(object sender, RoutedEventArgs e)
        {
            string table = TABLE.Text;
            string danji = ID_DANJI.Text;
            string build = ID_BUILD.Text;
            string house = ID_HOUSE.Text;
            string room = ID_ROOM.Text;
            

            string a = $"{table}\\{danji}\\{build}\\{house}\\{room}\\{CURTMP.IsChecked}\\{SETTMP.IsChecked}\\{ONFF.IsChecked}";

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
            ID_DANJI.Text = opts[0];
            ID_BUILD.Text = opts[1];
            ID_HOUSE.Text = opts[2];
            ID_ROOM.Text = opts[3];
        }
        #endregion

    }
}
