using InfluxDB.Client;
using InfluxDB.Client.Core.Flux.Domain;
using MONITOR_APP.MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MONITOR_APP.UTILITY
{
    static public class DB_influx
    {
        #region 참고용
        static async void f()
        {
            const string database = "ZIPSAI";
            const string retentionPolicy = "autogen";

            var client = InfluxDBClientFactory.Create("http://52.79.127.111:8086",
                "hsai", "han401#".ToCharArray());

            var query = $"from(bucket:\"{database}/{retentionPolicy}\") " +
                $" |> range(start: -15h)" +
                $" |> filter(fn: (r)=> r._measurement == \"sensor_data\")" +
                $" |> filter(fn: (r)=> r.DANJI_ID == \"2323\" and r.BUILD_ID ==\"202\")" +
                $" |> limit(n:1)";

            Console.WriteLine("start");
            var fluxTables = await client.GetQueryApi().QueryAsync(query, "org_id");
            fluxTables.ForEach(table => // 한 테이블
            {
                table.Records.ForEach(record => // 한 행
                {
                    Console.WriteLine($"{record.GetTime()} {record.GetMeasurement()}: {record.GetField()} {record.GetValue()} ... {record.Values["ROOM_ID"]} //");
                });
            });
            client.Dispose();
            Console.WriteLine("end");
        }
        static public InfluxDBClient GetClient()
        {
            return InfluxDBClientFactory.Create(
                "http://52.79.127.111:8086", "hsai", "han401#".ToCharArray());
        }
        static string GetQuery()
        {
            const string database = "ZIPSAI";
            const string retentionPolicy = "autogen";

            return $"from(bucket:\"{database}/{retentionPolicy}\")" +
                $" |> range(start: -20h)" +
                $" |> filter(fn: (r)=> r._measurement == \"sensor_data\")" +
                $" |> filter(fn: (r)=> r.DANJI_ID == \"2323\" and r.BUILD_ID == \"202\" and r.HOUSE_ID == \"101\" and r.ROOM_ID == \"0\")" +
                $" |> limit(n:10)";
        }
        #endregion

        static public InfluxDBClient GetClient(string ip, string port, string id, string pwd)
        {
            return InfluxDBClientFactory.Create(
                $"http://{ip}:{port}", id, pwd.ToCharArray());
        }
        static public async Task<List<FluxTable>> ExcuteInflux(InfluxDBClient client,string query)
        {
#if DEBUG
            Console.WriteLine($"[INFLUX] >> {query}");
#endif
            List<FluxTable> fluxTable = await client.GetQueryApi().QueryAsync(query, "org_id");
#if DEBUG
            Console.WriteLine($"result count is : {fluxTable.Count} * {fluxTable.FirstOrDefault()?.Records.Count}");
#endif

            client.Dispose();

            return fluxTable;
        }
        static public double GetData(FluxRecord record, string key)
        {
            if (record.GetField() == key) return Convert.ToDouble(record.GetValue());

            return double.NaN;
        }
        static public string GetData(List<FluxTable> tables, string key)
        {
            List<string> temp = new List<string>();
            List<int> num = new List<int>();
            foreach(var t in tables)
            {
                foreach(var d in t.Records)
                {
                    if (d.GetField() == key) //return Convert.ToString(d.GetValue());
                    {
                        if (temp.Contains(Convert.ToString(d.GetValue())))
                        {
                            int index = temp.IndexOf(Convert.ToString(d.GetValue()));
                            num[index]++;
                        }
                        else
                        {
                            temp.Add(Convert.ToString(d.GetValue()));
                            num.Add(1);
                        }
                    }
                }
            }
            if(temp.Count ==0)
                return "";
            else
            {
                int idx = num.IndexOf(num.Max());
                return temp[idx];
            }
        }

#region FLUX QUERY
        static public string GetQuery(SearchData s)
        {
            return $"from(bucket:\"ZIPSAI/autogen\")" +
                $" |> range(start: {s.mintime + 32400},stop: {s.maxtime+ 32400})" +
                $" |> filter(fn: (r)=> r._measurement == \"{s.TABLE}\")" +
                MakeQuery_Filter(s) +
                $" |> limit(n: {s.amount})";
        }
        static public string GetQuery_zero(SearchData s)
        {
            return $"from(bucket:\"ZIPSAI/autogen\")" +
                $" |> range(start: 0)" +
                $" |> filter(fn: (r)=> r._measurement == \"sensor_data\")" +
                MakeQuery_Filter(s);
        }
        static public string GetQuery_Search()
        {
            return $"from(bucket:\"ZIPSAI/autogen\")" +
                $" |> range(start: 0)" +
                $" |> filter(fn: (r)=> r._measurement == \"sensor_data\")" +
                $" |> filter(fn: (r)=> r.DANJI_ID != \"0\" and r.BUILD_ID != \"0\" and r.HOUSE_ID != \"0\")" +
                $" |> distinct(column: \"VALVE_STATUS\")";
        }
        static public string GetQuery_Group(SearchData s)
        {

            return $"from(bucket:\"ZIPSAI/autogen\")" +
                $" |> range(start: {s.mintime})" +
                $" |> filter(fn: (r)=> r._measurement == \"{s.TABLE}\")" +
                MakeQuery_Filter(s) +
                $" |> window(every: 1h, period: 30s)";
        }
        static private string MakeQuery_Filter(SearchData s)
        {
            string query = " |> filter(fn: (r) => r.SN != \"0\"";
            List<string> opt = new List<string>();
            if (s.DANJI_ID != "") opt.Add($" r.DANJI_ID ==\"{s.DANJI_ID}\"");
            if (s.BUILD_ID != "") opt.Add($" r.BUILD_ID ==\"{s.BUILD_ID}\"");
            if (s.HOUSE_ID != "") opt.Add($" r.HOUSE_ID ==\"{s.HOUSE_ID}\"");
            if (s.ROOM_ID != "") opt.Add($" r.ROOM_ID ==\"{s.ROOM_ID}\"");

            foreach(var v in opt)
            {
                if (v != "") query += " and " + v;
            }

            return query + " )";
        }
#endregion
    }
}
