﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MONITOR_APP.UTILITY
{
    static class TimeConverter
    {
        public static DateTime ConvertTimestamp(double timestamp)
        {
            DateTime converted = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            DateTime newDateTime = converted.AddSeconds(timestamp);
            return newDateTime.ToLocalTime();
        }
        public static string ConvertTimestamp_unix(int mon, int day, int yn, int hr, int min, int sec)
        {
            //create a new DateTime value based on the Unix Epoch
            DateTime converted = new DateTime(yn, mon, day, hr + 3, min, sec, 0);
            //return the value in string format
            return ((converted.Ticks - 621355968000000000) / 1000000).ToString();
        }
        public static string GetAll(string timestamp)
        {
            double d = Convert.ToDouble(timestamp);
            var val = TimeConverter.ConvertTimestamp(d);
            return $"{val.Year}/{val.Month}/{val.Day}[{val.Hour}:{val.Minute}:{val.Second}]";
        }
        public static string GetTime(string timestamp)
        {
            double d = Convert.ToDouble(timestamp);
            var val = TimeConverter.ConvertTimestamp(d);
            return $"[{val.Hour}:{val.Minute}:{val.Second}]";
        }
        public static string GetDate(string timestamp)
        {
            double d = Convert.ToDouble(timestamp);
            var val = TimeConverter.ConvertTimestamp(d);
            return $"{val.Year}.{val.Month}.{val.Day}";
        }
        public static string GetDate(double timestamp)
        {
            var val = TimeConverter.ConvertTimestamp(timestamp);
            return $"{val.Year}.{val.Month}.{val.Day}";
        }


    }
}
