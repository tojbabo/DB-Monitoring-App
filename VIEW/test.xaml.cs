
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Windows;

namespace MONITOR_APP.VIEW
{
    /// <summary>
    /// test.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class test : Window
    {
        public List<DataPoint> p { get; set; }
        public PlotModel mod { get; set; }

        public test()
        {
            InitializeComponent();
            p = new List<DataPoint>();
            mod = new PlotModel();
          

            for (int i = 0; i < 10000; i++)
            {
                p.Add(new DataPoint(i, i * i));
            }


           this.DataContext = this;

            LineSeries l = new LineSeries
            {
                Title = "sex",
                ItemsSource = p,
                DataFieldX = "x",
                DataFieldY = "y",
                StrokeThickness = 2,
                MarkerSize = 0,
                LineStyle = LineStyle.Solid,
                Color = OxyColors.Red,
                MarkerType = MarkerType.None,
            };

            mod.Series.Add(l);
        }
    }
}
