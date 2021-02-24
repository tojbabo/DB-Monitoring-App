﻿
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

            Thread t = new Thread(vm.ddd);
            t.Start();
            //vm.CreateSearchData();
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

        private void ListView_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = (SearchData)listview.SelectedItem;

            if (item == null) return;

            string opt = $"{item.DANJI_ID}\\{item.BUILD_ID}\\{item.HOUSE_ID}\\{item.ROOM_ID}";
            vm.RequestSelect(opt);  
        }
        private void Graph_RightDown(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem listboxitem = FindAncestor<ListBoxItem>
                                              ((DependencyObject)e.OriginalSource);
            
            if (listboxitem == null) return;
            try
            {
                ChartData content = (ChartData)(listboxitem.Content);
            vm.Vms.Remove(content);
            }
            catch(Exception err)
            {
                //Console.WriteLine($"{err}");
                return;
            }
        }
        private void Graph_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            // if (isDrag == true)
            // {

            ListBoxItem listboxitem = FindAncestor<ListBoxItem>
                                          ((DependencyObject)e.OriginalSource);
            if (listboxitem == null) return;

            ChartData content = (ChartData)(listboxitem.Content);

            content.selected = !content.selected;

            int index = vm.Vms.IndexOf(content);

            vm.Vms.Remove(content);
            content.ReFresh();

            vm.Vms.Insert(index, content);


            //content.ReFresh();
            //  }
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

                vm.Vms.Move(indexDrag, index);

                content.ReFresh();

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

        public void GridTurnOnOff()
        {
            Grid_side.Visibility = (Grid_side.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
            Col.Width = (Grid_side.Visibility == Visibility.Visible) ? new GridLength(/*1,GridUnitType.Star*/130) : GridLength.Auto;
            Grid_search.Visibility = Visibility.Collapsed;
        }
        // 리스트 박스 자체 왼쪽 클릭
        private void ListBox_LeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDrag == true)
            {
                ListBoxItem listboxitem = FindAncestor<ListBoxItem>
                                                  ((DependencyObject)e.OriginalSource);
                if (listboxitem == null) return;

                ChartData content = (ChartData)(listboxitem.Content);

                content.selected = !content.selected;

                int index = vm.Vms.IndexOf(content);

                vm.Vms.Remove(content);
                vm.Vms.Insert(index, content);

                content.ReFresh();
            }
        }
        // 리스트 박스, 디테일 창 내 체크박스 이벤트
        private void Checked_Graph(object sender, RoutedEventArgs e)
        {
            var c = e.OriginalSource as CheckBox;
            var datacontext = c?.DataContext as ChartData;
            ChartData content = (ChartData)datacontext;

            int index = vm.Vms.IndexOf(content);

            vm.Vms.Remove(content);
            content.ReFresh();

            vm.Vms.Insert(index, content);
        }

        private void Button_Expand(object sender, RoutedEventArgs e)
        {
            ListBoxItem listboxitem = FindAncestor<ListBoxItem>
                                              ((DependencyObject)e.OriginalSource);
            if (listboxitem == null) return;

            ChartData content = (ChartData)(listboxitem.Content);
            content.selected = !content.selected;

            int index = vm.Vms.IndexOf(content);

            vm.Vms.Remove(content);
            content.ReFresh();

            vm.Vms.Insert(index, content);

        }

        private void Button_Delete(object sender, RoutedEventArgs e)
        {
            ListBoxItem listboxitem = FindAncestor<ListBoxItem>
                                              ((DependencyObject)e.OriginalSource);

            if (listboxitem == null) return;

            ChartData content = (ChartData)(listboxitem.Content);

            vm.Vms.Remove(content);
        }

        private void Button_Select(object sender, MouseButtonEventArgs e)
        {
            if (isDrag == false) isDrag = true;
        }
        private void Button_Reload(object sender, RoutedEventArgs e)
        {
            var a = sender as Button;
            var b = (ChartData)a.DataContext;

            Console.WriteLine($"result is : {TimeConverter.GetDate( b.searches.mintime)} - {TimeConverter.GetDate( b.searches.maxtime)} - {b.searches.interval}");
        }

        bool istile = false;
        public void Tiling()
        {
            var a = new ObservableCollection<ChartData>(vm.Vms);
            vm.Vms.Clear();
            
            if (istile == true)
            {
                FrameworkElementFactory factory = new FrameworkElementFactory(typeof(StackPanel));
                ItemsPanelTemplate template = new ItemsPanelTemplate();
                template.VisualTree = factory;
                listBox.ItemsPanel = template;
                istile = false;
            }
            else
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
    }
}
