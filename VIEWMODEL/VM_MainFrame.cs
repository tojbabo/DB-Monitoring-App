﻿using InfluxDB.Client;
using MONITOR_APP.UTILITY;
using MONITOR_APP.VIEW;
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
        StaticSetWindow SSW;
        BASE head;
        public VM_MainFrame()
        {
            head = BASE.getBASE();
            //head.newConnect("52.79.127.111", "3306", "hansung_db", "hansung", "aidb4231@");
            head.newClient();
#if DEBUG
            NativeMethods.AllocConsole();
#endif
        }

        public void DBConnectSetting()
        {
            DBConnectWindow DBW = new DBConnectWindow();
            DBW.Show();
        }

#if DEBUG
        static class NativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool AllocConsole();
        }
#endif

        public void StaticValueSetting()
        {
            SSW = new StaticSetWindow();
            SSW.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            SSW.Topmost = true;
            SSW.OnChildTextInputEvent += new StaticSetWindow.OnChildTextInputHandler(SSW_ChildEvent);
            SSW.Show();
        }

        private void SSW_ChildEvent(object p)
        {
            SSW.Close();
            SSW.OnChildTextInputEvent -= new StaticSetWindow.OnChildTextInputHandler(SSW_ChildEvent);
            SSW = null;

            head.getMV_MainPage().Chart_Reload();
        }
    }
}
