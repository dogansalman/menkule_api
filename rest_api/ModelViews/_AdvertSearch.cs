using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using rest_api.Models;

namespace rest_api.ModelViews
{
    public class User
    {
        public int id { get; set; }
        public string name { get; set; }
        public string lastname { get; set; }
        public string photo { get; set; }
        
    }

    public class _AdvertSearch 
    {
        public string adress { get; set; }
        public string title { get; set; }
        public string entry_time { get; set; }
        public string exit_time { get; set; }
        public bool state { get; set; }
        public int views { get; set; }
        public int cancel_time { get; set; }
        public int min_layover { get; set; }
        public int comment_size { get; set; }
        public int score { get; set; }
        public decimal price { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public Cities city { get; set; }
        public Towns town { get; set; }
        public AdvertTypes advert_type { get; set; }
        public AdvertProperties properties{ get; set; }
        public AdvertPossibilities possibility { get; set; }
        public ICollection<AdvertUnavailableDate> unavaiable_date { get; set; }
        public ICollection<AdvertAvailableDate> available_date { get; set; }
        public object user { get; set; }
        public DateTime created_date { get; set; }
    }
}