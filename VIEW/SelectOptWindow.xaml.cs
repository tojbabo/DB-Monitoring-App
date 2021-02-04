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


        #region Event

        private void Buton_OK(object sender, RoutedEventArgs e)
        {
            string table = TABLE.Text;
            string danji = ID_DANJI.Text;
            string build = ID_BUILD.Text;
            string house = ID_HOUSE.Text;
            string room = ID_ROOM.Text;
           

            string a = $"{table}\\{danji}\\{build}\\{house}\\{room}\\ABC";
            

            if (OnChildTextInputEvent != null) OnChildTextInputEvent(a);

        }
        private void GridMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
            {
                this.WindowState = (this.WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
            }

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

        private void Close(object sender, RoutedEventArgs e)
        {
            if (OnChildTextInputEvent != null) OnChildTextInputEvent(null);
            this.Close();
        }
    }
}
