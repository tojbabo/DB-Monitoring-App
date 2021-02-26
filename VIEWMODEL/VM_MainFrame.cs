using InfluxDB.Client;
using MONITOR_APP.UTILITY;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MONITOR_APP.VIEWMODEL
{
    public class VM_MainFrame
    {
        BASE head;
        public VM_MainFrame()
        {
            head = BASE.getBASE();
            head.newConnect("52.79.127.111", "3306", "hansung_db", "hansung", "aidb4231@");
            head.newClient("52.79.127.111", "8086", "hsai", "han401#");
#if DEBUG
            NativeMethods.AllocConsole();
#endif
        }

        public async void test()
        {
            InfluxDBClient client = DB_influx.GetClient();

            var query = $"from(bucket:\"ZIPSAI/autogen\")" +
                $" |> range(start: 0)" +
                $" |> filter(fn: (r)=> r._measurement == \"{"sensor_data"}\" and r.DANJI_ID == \"2323\" and r.BUILD_ID == \"202\" and r.HOUSE_ID == \"101\" and r.ROOM_ID == \"0\")";


            var tables = await DB_influx.ExcuteInflux(client, query);
            Console.WriteLine($"good");

            tables.ForEach(record =>
            {
                var cell = record.Records[0];

                Console.WriteLine($"[{record.Records[0].Values["DANJI_ID"]}/{record.Records[0].Values["BUILD_ID"]}" +
                    $"/{record.Records[0].Values["HOUSE_ID"]}/{record.Records[0].Values["ROOM_ID"]}] .. {record.Records[0].Values["SN"]}");

                Console.WriteLine($"detail is : {cell.GetTime()} {cell.GetMeasurement()}: {cell.GetField()} {cell.GetValue()}");
            });
        }

#if DEBUG
        static class NativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool AllocConsole();
        }
#endif
    }
}
