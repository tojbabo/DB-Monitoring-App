using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MONITOR_APP.UTILITY
{
    static public class MySQL
    {
        static MySqlConnection connection = null;
        static bool DataSaveResult;
        static bool DataSearchResult;


        static public void Connect()
        {
            try
            {
                string connectionPath = $"SERVER=127.0.0.1;" +
                    $"DATABASE=profile;" +
                    $"UID=root;" +
                    $"PASSWORD=1234;";
                connection = new MySqlConnection(connectionPath);
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
                    $"PASSWORD={pass};";
                connection = new MySqlConnection(connectionPath);
                DBOpen();
                DBClose();

                return connection;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        // DataBase Connection
        static private bool DBOpen()
        {
            try
            {
                connection.Open();
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
        static private bool DBClose()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException e)
            {
                //Debug.WriteLine(e.Message);
                return false;
            }
        }

        // Queyr Executer(Insert, Delete, Update ...)
        static public void MySqlQueryExecuter(string userQuery)
        {
            if (DBOpen() == true)
            {
                MySqlCommand command = new MySqlCommand(userQuery, connection);

                if (command.ExecuteNonQuery() == 1)
                {
                    //Debug.WriteLine("값 저장 성공");
                    DataSaveResult = true;
                }
                else
                {
                    //Debug.WriteLine("값 저장 실패");
                    DataSaveResult = false;
                }

                DBClose();
            }
        }

        static public void ggg(MySqlConnection conn, string table)
        {
            string query = $"SELECT * FROM {table};";

            if (DBOpen() == true)
            {
                MySqlCommand command = new MySqlCommand(query, conn);
                MySqlDataReader dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    element[0].Add(dataReader["id"].ToString());
                    element[1].Add(dataReader["pw"].ToString());
                }

                // 추가된 코드
                if (element != null)
                {
                    for (int i = 0; i < element[0].Count; i++)
                    {
                        if (element[0][i].Contains(id))
                        {
                            for (int j = 0; j < element[1].Count; i++)
                            {
                                if (element[1][i].Contains(pw))
                                {
                                    DataSearchResult = true;
                                    break;
                                }
                            }
                        }
                        break;
                    }
                }

                dataReader.Close();
                DBClose();

                return element;
            }
            else
            {
                return null;
            }
        }

        static public List<string>[] Select(string tableName, int columnCnt, string id, string pw)
        {
            string query = "SELECT * FROM" + " " + tableName;

            List<string>[] element = new List<string>[columnCnt];

            for (int index = 0; index < element.Length; index++)
            {
                element[index] = new List<string>();
            }

            if (DBOpen() == true)
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    element[0].Add(dataReader["id"].ToString());
                    element[1].Add(dataReader["pw"].ToString());
                }

                // 추가된 코드
                if (element != null)
                {
                    for (int i = 0; i < element[0].Count; i++)
                    {
                        if (element[0][i].Contains(id))
                        {
                            for (int j = 0; j < element[1].Count; i++)
                            {
                                if (element[1][i].Contains(pw))
                                {
                                    DataSearchResult = true;
                                    break;
                                }
                            }
                        }
                        break;
                    }
                }

                dataReader.Close();
                DBClose();

                return element;
            }
            else
            {
                return null;
            }
        }
    }
}
