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
       // public double interval { get; set; }

        public int index;
       

        public double amount;


        public SearchData() { mintime = -1; maxtime = -1; index = -1; }
    }

}
