using MONITOR_APP.MODEL;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MONITOR_APP.UTILITY
{
    static public class DB_mysql
    {
        static private double minTime = 1606271503;
        static public void Connect()
        {
            try
            {
                string connectionPath = $"SERVER=127.0.0.1;" +
                    $"DATABASE=profile;" +
                    $"UID=root;" +
                    $"PASSWORD=1234;";
                MySqlConnection connection = new MySqlConnection(connectionPath);
                connection.Open();
                connection.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        static public MySqlConnection Connect(string ip, string port, string db_name, string id, string pass)
        {
            try
            {
                string connectionPath = $"SERVER={ip};" +
                    $"Port={port};" +
                    $"DATABASE={db_name};" +
                    $"UID={id};" +
                    $"PASSWORD={pass};" +
                    $"Connection Timeout=60";
                MySqlConnection connection = new MySqlConnection(connectionPath);
                DBOpen(connection);
                DBClose(connection);


                return connection;
            }
            catch
            {
                return null;
            }
        }

        // DataBase Connection
        static private bool DBOpen(MySqlConnection conn)
        {
            try
            {
                conn.Open();
                return true;
            }
            catch (MySqlException e)
            {
                switch (e.Number)
                {
                    case 0:
                        //Debug.WriteLine("Unable to Connect to Server");
                        break;
                    case 1045:
                        //Debug.WriteLine("Please check your ID or PassWord");
                        break;
                }
                return false;
            }
        }

        // DataBase Close
        static private bool DBClose(MySqlConnection conn)
        {
            try
            {
                conn.Close();
                return true;
            }
            catch (MySqlException e)
            {
                //Debug.WriteLine(e.Message);
                return false;
            }
        }

        // Queyr Executer(Insert, Delete, Update ...)
        static public void MySqlQueryExecuter(MySqlConnection conn, string userQuery)
        {
            if (DBOpen(null) == true)
            {
                MySqlCommand command = new MySqlCommand(userQuery, conn);

                if (command.ExecuteNonQuery() == 1)
                {
                    //Debug.WriteLine("값 저장 성공");
                }
                else
                {
                    //Debug.WriteLine("값 저장 실패");
                }

                DBClose(null);
            }
        }

        static public DataTable SelectTable(MySqlConnection conn, string query)
        {
            Console.WriteLine($"[MYSQL] >> {query}");
            if (DBOpen(conn) == true)
            {
                try
                {
                    MySqlCommand command = new MySqlCommand(query, conn);

                    command.CommandTimeout = 120;

                    DataTable dt = new DataTable();
                    MySqlDataAdapter da = new MySqlDataAdapter(command);

                    da.Fill(dt);

                    DBClose(conn);
                    return dt;
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Select Error!\n{e.Message}");

                    DBClose(conn);
                    return null;
                }

            }
            else
            {
                return null;
            }
        }

        static public string MakeQuery(object obj)
        {
            SearchData s = (SearchData)obj;
            string table = s.TABLE;

            string query = $"SELECT * FROM {table} WHERE ID IS NOT NULL ";

            return query +$"{make_WHERE(s)};";
        }

        static public string SearchQuery(string table)
        {
            //string query = $"SELECT DISTINCT DANJI_ID, BUILD_ID, HOUSE_ID, ROOM_ID FROM {table};";
            string query = $"SELECT DANJI_ID, BUILD_ID, HOUSE_ID, ROOM_ID, COUNT(*) " +
                $"FROM {table} " +
                $"WHERE DANJI_ID='2323' AND BUILD_ID='202' AND TIME > {minTime} " +
                $"GROUP BY DANJI_ID,BUILD_ID,HOUSE_ID,ROOM_ID";
            return query;
        }

        static public string make_WHERE(SearchData s)
        {
            string sql = "";

            sql += (s.DANJI_ID == "") ? "" : $" AND DANJI_ID = {s.DANJI_ID}";
            sql += (s.BUILD_ID == "") ? "" : $" AND BUILD_ID = {s.BUILD_ID}";
            sql += (s.HOUSE_ID == "") ? "" : $" AND HOUSE_ID = {s.HOUSE_ID}";
            sql += (s.ROOM_ID == "") ? "" : $" AND ROOM_ID = {s.ROOM_ID}";

            sql += (s.mintime == 0) ? "" : $" AND TIME > {s.mintime}";
            sql += (s.maxtime == 0) ? "" : $" AND TIME < {s.maxtime}";
            int interv = 0;
            if (s.interval == 0) interv = 6;
            else if (s.interval == 1) interv = 30;
            else if (s.interval == 2) interv = 60;
            else if (s.interval == 3) interv = 360;

            sql += (interv == 0) ? "" : $" AND mod(substr(TIME, 7, 3),{interv}) <= 2";

            return sql;
        }
    }
}
