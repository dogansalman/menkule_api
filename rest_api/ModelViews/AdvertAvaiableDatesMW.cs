using System;

namespace rest_api.ModelViews
{
    public class AdvertAvaiableDatesMW
    {
        public DateTime from { get; set; }
        public int from_day { get; set; }
        public DateTime from_fulldate { get; set; }
        public int from_month { get; set; }
        public int from_year { get; set; }
        public DateTime to { get; set; }
        public int to_day { get; set; }
        public DateTime to_fulldate { get; set; }
        public int to_month { get; set; }
        public int to_year { get; set; }
        public string uniq { get; set; }
    }
}