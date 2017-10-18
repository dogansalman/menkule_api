using System;
using System.Collections.Generic;
using rest_api.Models;
namespace rest_api.ModelViews
{
    public class _Advert
    {
        public int id { get; set; }
        public int advert_type_id { get; set; }
        public string adress { get; set; }
        public int city_id { get; set; }
        public int town_id { get; set; }
        public int user_id { get; set; }
        public string entry_time { get; set; }
        public string exit_time { get; set; }
        public bool state { get; set; } = false;
        public int views { get; set; }
        public int score { get; set; }
        public decimal price { get; set; }
        public int min_layover { get; set; }
        public int cancel_time { get; set; }
        public string description { get; set; }
        public int zoom { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string title { get; set; }
        public DateTime created_date { get; set; } = DateTime.Now;
        public DateTime? updated_date { get; set; }
        public object images { get; set; }
        public AdvertProperties properties { get; set; }
        public AdvertPossibilities possibility { get; set; }
        public ICollection<AdvertUnavailableDate> unavaiable_date { get; set; }
        public ICollection<AdvertAvailableDate> available_date { get; set; }
    }
}