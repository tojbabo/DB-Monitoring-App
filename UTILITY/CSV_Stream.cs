using MONITOR_APP.MODEL;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MONITOR_APP.UTILITY
{
    static public class CSV_Stream
    {
        static private string _path = @"output";
        static public void CSV_Write(ChartData chart)
        {
            DirectoryInfo dir = new DirectoryInfo(_path);
            if (dir.Exists == false)
            {
                dir.Create();
            }
            string filename = $"{chart.searches.DANJI_ID}_{chart.searches.BUILD_ID}_{chart.searches.HOUSE_ID}_{chart.searches.ROOM_ID}" +
                $"[{TimeConverter.GetDate(chart.searches.mintime)}~{TimeConverter.GetDate(chart.searches.maxtime)}].csv";
#if DEBUG
            Console.WriteLine($"{_path}~{filename}");
#endif
            using (StreamWriter stream = new StreamWriter(_path + @"\" + filename,false, Encoding.Default))
            {
                string time;
                double cur, set;
                bool onff;
                stream.WriteLine("CUR_TEMP,SET_TEMP,VAVLE_STATUS,TIME");
                for (int i = 0; i < chart.set.Count; i++)
                {
                    cur = chart.cur[i].Y;
                    set = chart.set[i].Y;
                    onff = (chart.onff[i].Y == 5) ? true : false;
                    time = DateTimeAxis.ToDateTime(chart.cur[i].X).ToString();
                    try
                    {
                        stream.WriteLine($"{cur},{set},{onff},{time}");
                    }catch(Exception e)
                    {
                        break;
                    }
                }

                Process.Start(_path);
            }
        }
    }
}
