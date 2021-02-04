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
    static public class MySQL
    {
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
        static public void MySqlQueryExecuter(MySqlConnection conn,string userQuery)
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

        static public string MakeQuery(string[] opts)
        {
            string table = opts[0];
            string danji = opts[1];
            string build = opts[2];
            string house = opts[3];
            string room = opts[4];

            string query = $"SELECT * FROM {table} WHERE ID IS NOT NULL ";

            if (danji != "") danji = $" AND DANJI_ID = '{danji}'";
            if (build != "") build = $" AND BUILD_ID = '{build}'";
            if (house != "") house = $" AND HOUSE_ID = '{house}'" ;
            if (room != "") room = $" AND ROOM_ID = '{room}'";

            query = query + danji + build + house + room + " limit 100;";

            return query;
        }

        static public string SearchQuery(string table)
        {
            //string query = $"SELECT DISTINCT DANJI_ID, BUILD_ID, HOUSE_ID, ROOM_ID FROM {table};";
            string query = $"SELECT DANJI_ID, BUILD_ID, HOUSE_ID, ROOM_ID, COUNT(*) " +
                $"FROM {table} " +
                $"WHERE DANJI_ID='2323' AND BUILD_ID='202' " +
                $"GROUP BY DANJI_ID,BUILD_ID,HOUSE_ID,ROOM_ID";
            return query;
        }

    }
}
