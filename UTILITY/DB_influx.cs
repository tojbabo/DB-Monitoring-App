using InfluxDB.Client;

using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core;
using InfluxDB.Client.Writes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MONITOR_APP.UTILITY
{
    static public class DB_influx
    {
        private static readonly char[] Token = "".ToCharArray();
        static async public void f()
        {
            const string database = "ZIPSAI";
            const string retentionPolicy = "autogen";

            var client = InfluxDBClientFactory.CreateV1("http://52.79.127.111:8086",
                "hsai",
                "han401#".ToCharArray(),
                database,
                retentionPolicy);


            var query = $"from(bucket: \"{database}/{retentionPolicy}\") |> range(start: -1h)";
            var fluxTables = await client.GetQueryApi().QueryAsync(query);
            var fluxRecords = fluxTables[0].Records;
            fluxRecords.ForEach(record =>
            {
                Console.WriteLine($"{record.GetTime()} {record.GetMeasurement()}: {record.GetField()} {record.GetValue()}");
            });

            client.Dispose();
        }

        static public void g()
        {
            f();
        }
    }
}
