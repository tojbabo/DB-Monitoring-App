﻿using MONITOR_APP.UTILITY;
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
            if(conn == null) conn = MySQL.Connect("52.79.127.111", "3306", "hansung_db", "hansung", "aidb4231@");
       
            return conn;
        }
        public MySqlConnection getConnect()
        {
            return conn;
        }

        private BASE()
        {
            conn = null;
        }
        private void Initalize()
        {
            mv_mainframe = new VM_MainFrame();
            mv_mainpage = new VM_MainPage();
        }

        
    }
}
