namespace MONITOR_APP.MODEL
{
    public class SearchData
    {
        public string TABLE { get; set; }

        public string DANJI_ID { get; set; }
        public string BUILD_ID { get; set; }
        public string HOUSE_ID { get; set; }
        public string ROOM_ID { get; set; }
        

        public bool tmp_cur { get; set; }
        public bool tmp_set { get; set; }
        public bool on_off { get; set; }

        public double mintime;
        public double maxtime;
        public double interval { get; set; }

       

        public double amount;

        public string make_WHERE()
        {
            string sql = "";

            sql += (DANJI_ID == "") ? "" : $" AND DANJI_ID = {DANJI_ID}";
            sql += (BUILD_ID == "") ? "" : $" AND BUILD_ID = {BUILD_ID}";
            sql += (HOUSE_ID == "") ? "" : $" AND HOUSE_ID = {HOUSE_ID}";
            sql += (ROOM_ID == "") ? "" : $" AND ROOM_ID = {ROOM_ID}";

            sql += (mintime == 0) ? "" : $" AND TIME > {mintime}";
            sql += (maxtime == 0) ? "" : $" AND TIME < {maxtime}";
            int interv = 0;
            if (interval == 0) interv = 6;
            else if (interval == 1) interv = 30;
            else if (interval == 2) interv = 60;
            else if (interval == 3) interv = 360;

            sql += (interv == 0) ? "" : $" AND mod(substr(TIME, 7, 3),{interv}) <= 2";

            return sql;
        }

    }

}
