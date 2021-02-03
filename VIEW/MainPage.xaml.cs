using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Wpf.Charts.Base;
using MONITOR_APP.MODEL;
using MONITOR_APP.UTILITY;
using MONITOR_APP.VIEWMODEL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Permissions;
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
    /// MainPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainPage : Page
    {
        BASE head;
        MV_MainPage vm;
        
        public MainPage()
        {
            InitializeComponent();
            head = BASE.getBASE();

            vm = head.getMV_MainPage();
            this.DataContext = vm;            

            



        }

        #region Event

        private void Button_ADD(object sender, RoutedEventArgs e)
        {
            vm.RequestSelect();
        }

        private void Button_CLEAR(object sender, RoutedEventArgs e)
        {
            vm.ChartReset();
        }

        #endregion

        #region  drag & drop

        private bool isDrag = false;
        private int indexDrag = -1;

        private void ListBox_LeftClick(object sender, MouseButtonEventArgs e)
        {
            var temp = FindAncestor<ListBoxItem>
                ((DependencyObject)(listBox.InputHitTest(e.GetPosition(listBox))));
            if (temp != null)
                isDrag = true;
        }

        private void ListBox_Drag(object sender, MouseEventArgs e)
        {
            if ((e.LeftButton == MouseButtonState.Pressed) && isDrag == true)
            {
                ListBoxItem listboxitem = FindAncestor<ListBoxItem>
                                                ((DependencyObject)e.OriginalSource);

                if (listboxitem == null)
                    return;

                object item = listboxitem.DataContext;

                indexDrag = listBox.Items.IndexOf(item);


                ChartData content = (ChartData)(listboxitem.Content);

                DataObject data = new DataObject("MOVE", content);
                DragDrop.DoDragDrop(listboxitem, data, DragDropEffects.Move);
            }

        }

        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("MOVE"))
            {
                ChartData content = e.Data.GetData("MOVE") as ChartData;


                int index = -1;
                var temp = FindAncestor<ListBoxItem>
                                ((DependencyObject)(listBox.InputHitTest(e.GetPosition(listBox))));

                
                object item = temp?.DataContext;

                if (temp == null)
                    index = listBox.Items.Count - 1;
                else
                    index = listBox.Items.IndexOf(item);


                var something = vm.Vms[indexDrag];

                vm.Vms.Remove(something);
                if (index < indexDrag) indexDrag--;

                vm.Vms.Insert(index, something);


                indexDrag = -1;
                isDrag = false;
            }
        }

        private static T FindAncestor<T>(DependencyObject obj)
            where T : DependencyObject
        {
            do
            {
                if (obj is T)
                {
                    return (T)obj;
                }
                obj = VisualTreeHelper.GetParent(obj);
            }
            while (obj != null);
            return null;
        }


        #endregion

        private void ListBox_LeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(isDrag == true)
            {
                ListBoxItem listboxitem = FindAncestor<ListBoxItem>
                                                  ((DependencyObject)e.OriginalSource);

                ChartData content = (ChartData)(listboxitem.Content);

                vm.GetDetailChart(content);

            }

        }


        public void GridTurnOnOff()
        {
            Grid_side.Visibility = (Grid_side.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
            Col.Width = (Grid_side.Visibility == Visibility.Visible) ? new GridLength(1,GridUnitType.Star) : GridLength.Auto;
        }
    }
}
