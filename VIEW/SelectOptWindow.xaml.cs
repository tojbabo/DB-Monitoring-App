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

        private void Buton_OK(object sender, RoutedEventArgs e)
        {
            string a = $"{TABLE.Text}\\{ID_DANJI.Text}\\*\\*\\*\\ABC";
            

            if (OnChildTextInputEvent != null) OnChildTextInputEvent(a);

        }
    }
}
