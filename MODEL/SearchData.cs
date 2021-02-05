using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MONITOR_APP.MODEL
{
    public class SearchData
    {
        public string danji { get; set; }
        public string build { get; set; }
        public string house { get; set; }
        public string room { get; set; }
        

        public bool tmp_cur { get; set; }
        public bool tmp_set { get; set; }
        public bool on_off { get; set; }

        public SearchData() { }
        public SearchData(SearchData sd)
        {
            danji = sd.danji;
            build = sd.build;
            house = sd.house;
            room = sd.room;

            tmp_cur = sd.tmp_cur;
            tmp_set = sd.tmp_set;
            on_off = sd.on_off;
        }
       
    }
}
