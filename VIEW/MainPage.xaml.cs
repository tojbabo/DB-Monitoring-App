
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
using System.Threading;
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
        readonly BASE head;
        readonly VM_MainPage vm;
        
        public MainPage()
        {
            InitializeComponent();
            head = BASE.getBASE();

            vm = head.getMV_MainPage();
            this.DataContext = vm;

            Thread t = new Thread(vm.CreateSearchData_Influx);
            t.Start();
            //vm.CreateSearchData();
        }
        public void GridTurnOnOff()
        {
            Grid_side.Visibility = (Grid_side.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
            Col.Width = (Grid_side.Visibility == Visibility.Visible) ? new GridLength(/*1,GridUnitType.Star*/130) : GridLength.Auto;

            Grid_search.Visibility = Visibility.Collapsed;
        }
        bool istile = false;
        public void Tiling(e_bool e = e_bool.manual)
        {
            var a = new ObservableCollection<ChartData>(vm.Vms);
            vm.Vms.Clear();

            if ((istile == true && e == e_bool.manual) || e == e_bool.True)
            {
                FrameworkElementFactory factory = new FrameworkElementFactory(typeof(StackPanel));
                ItemsPanelTemplate template = new ItemsPanelTemplate();
                template.VisualTree = factory;
                listBox.ItemsPanel = template;
                istile = false;
            }
            else if ((istile == false && e == e_bool.manual) || e == e_bool.False)
            {
                FrameworkElementFactory factory = new FrameworkElementFactory(typeof(WrapPanel));
                factory.SetValue(WrapPanel.OrientationProperty, Orientation.Horizontal);
                ItemsPanelTemplate template = new ItemsPanelTemplate();
                template.VisualTree = factory;
                listBox.ItemsPanel = template;
                istile = true;
            }

            foreach (var item in a)
            {
                item.ReFresh();
                vm.Vms.Add(item);
            }
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
        private void Button_SEARCH(object sender, RoutedEventArgs e)
        {
            //vm.f();
            Grid_search.Visibility = (Grid_search.Visibility == Visibility.Visible)?Visibility.Collapsed:Visibility.Visible;
        }
        private void Button_MODIFY(object sender, RoutedEventArgs e)
        {
            vm.Chart_Modify();
        }
        private void Button_Remove(object sender, RoutedEventArgs e)
        {
            var d = sender as Button;
            var datacontext = d?.DataContext as ChartData;
            ChartData content = (ChartData)datacontext;

            vm.ChartReset(content);
        }
        private void ListView_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = (SearchData)listview.SelectedItem;

            if (item == null) return;

            vm.RequestSelect(item,this);  
        }
        private void ListView_RightUp(object sender, MouseButtonEventArgs e)
        {
            var v = listview.SelectedItems.Cast<SearchData>().ToList();

            if (v == null) return;

            vm.RequestSelect(v,this);
        }
        private void Graph_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            // if (isDrag == true)
            // {

            ListBoxItem listboxitem = FindAncestor<ListBoxItem>
                                          ((DependencyObject)e.OriginalSource);
            if (listboxitem == null) return;

            if (!(listboxitem.Content is ChartData)) return;
            
            
            ChartData content = (ChartData)(listboxitem.Content);

            content.selected = !content.selected;

            int index = vm.Vms.IndexOf(content);

            vm.Vms.Remove(content);
            content.ReFresh();

            vm.Vms.Insert(index, content);


            //content.ReFresh();
            //  }
        }
        private void Button_Reload(object sender, RoutedEventArgs e)
        {
            var a = sender as Button;
            var b = (ChartData)a.DataContext;

            vm.Chart_Reload(b);
        }
        #endregion

        #region  drag & drop

        private bool isDrag = false;
        private int indexDrag = -1;
        
        // 리스트 박스 단순 왼쪽 클릭 - 리스트 박스 요소가 아닌 그래프를 클릭해야 반응
        private void Graph_LeftDown(object sender, MouseButtonEventArgs e)
        {
            var temp = FindAncestor<ListBoxItem>
                ((DependencyObject)(listBox.InputHitTest(e.GetPosition(listBox))));
            if (temp != null)
                isDrag = true;
        }
        
        // 리스트 박스 위에서 마우스 움직임 감지
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

        // 드래그 중인 요소에 한해 드랍 이벤트
        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("MOVE"))
            {
                ChartData content = e.Data.GetData("MOVE") as ChartData;

                if (indexDrag == -1) return;
                   
                int index;
                var temp = FindAncestor<ListBoxItem>
                                ((DependencyObject)(listBox.InputHitTest(e.GetPosition(listBox)))); 

                ChartData item = (ChartData)temp?.DataContext;

                if (temp == null)
                    index = listBox.Items.Count - 1;
                else
                    index = listBox.Items.IndexOf(item);

                vm.Vms.Remove(content);
                content.ReFresh();
                vm.Vms.Insert(index, content);

                indexDrag = -1;
                isDrag = false;
            }
        }

        // 리스트 박스위에서 이벤트가 발생했을때 해당 요소를 찾는 함수
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


    }
}
