using InfluxDB.Client;
using MONITOR_APP.MODEL;
using MONITOR_APP.UTILITY;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MONITOR_APP.VIEWMODEL
{
    public sealed class BASE
    {
        private static BASE staticBASE = null;
        public static BASE getBASE()
        {
            if (staticBASE == null)
            {
                staticBASE = new BASE();
                staticBASE.Initalize();
            }
            return staticBASE;
        }

        private VM_MainFrame mv_mainframe;
        public VM_MainFrame getMV_MainFrame()
        {
            return mv_mainframe;
        }

        private VM_MainPage mv_mainpage;
        public VM_MainPage getMV_MainPage()
        { 
            return mv_mainpage;
        }

        private MySqlConnection conn;
        public MySqlConnection newConnect(string ip, string port, string db_name, string id, string pass)
        {
            if(conn == null) conn = DB_mysql.Connect(ip, port, db_name, id, pass);
       
            return conn;
        }
        public MySqlConnection getConnect()
        {
            return conn;
        }

        private InfluxDBClient influxDBClient;
        public InfluxDBClient newClient()
        {
            influxDBClient = DB_influx.GetClient(dbDetail.ip, dbDetail.port, dbDetail.uid, dbDetail.passwd);
            return influxDBClient;
        }
        public InfluxDBClient getClient()
        {
            if (influxDBClient == null) return null;
            return influxDBClient;
        }

        public DBConnection dbDetail;

        private BASE()
        {
            conn = null;
            dbDetail = new DBConnection();
            dbDetail.ip = "52.79.127.111";
            dbDetail.port = "8086";
            dbDetail.uid = "hsai";
            dbDetail.passwd = "han401#";
        }
        private void Initalize()
        {
            mv_mainframe = new VM_MainFrame();
            mv_mainpage = new VM_MainPage();
        }
    }
}
