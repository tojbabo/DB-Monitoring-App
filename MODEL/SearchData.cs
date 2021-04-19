namespace MONITOR_APP.MODEL
{
    public class SearchData
    {
        public string TABLE { get; set; }

        public string DANJI_ID { get; set; }
        public string BUILD_ID { get; set; }
        public string HOUSE_ID { get; set; }
        public string ROOM_ID { get; set; }
        

        public double mintime;
        public double maxtime;
       // public double interval { get; set; }

        public int index;
       

        public double amount;


        public SearchData() { mintime = -1; maxtime = -1; index = -1; }
        public SearchData(SearchData d)
        {
            TABLE = d.TABLE;
            DANJI_ID = d.DANJI_ID;
            BUILD_ID = d.BUILD_ID;
            HOUSE_ID = d.HOUSE_ID;
            ROOM_ID = d.ROOM_ID;
            mintime = d.mintime;
            maxtime = d.maxtime;
            index = d.index;
            amount = d.amount;
        }
    }

}
