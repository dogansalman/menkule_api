using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using rest_api.Models;

namespace rest_api.ModelViews
{
    public class _RezervationAdvert
    {
        public RezervationAdverts advert { get; set; }
        public AdvertPossibilities possibilities { get; set; }
        public AdvertProperties properties { get; set; }
        public Cities cities { get; set; }
        public Towns towns { get; set; }
        public AdvertTypes advert_type { get; set; }
    }
    public class _RezervationUserInfo
    {
        public string fullname { get; set; }
        public string gsm { get; set; }
        public string photo { get; set; }
    }
    public class _RezervationDetails
    {
   
        public int id { get; set; }
        public string note { get; set; }
        public int user_id { get; set; }
        public int owner { get; set; }
        public int visitor { get; set; }
        public int advert_id { get; set; }
        public int days { get; set; }
        public decimal day_price { get; set; }
        public decimal total_price { get; set; }
        public DateTime checkin { get; set; }
        public DateTime checkout { get; set; }
        public string description_state { get; set; }
        public bool state { get; set; }
        public bool is_cancel { get; set; }
        public DateTime created_date { get; set; } 
        public DateTime? updated_date { get; set; }
        public _RezervationAdvert rezervation_advert { get; set; }
        public List<RezervationVisitors> visitors { get; set; }
        public _RezervationUserInfo user_information { get; set; }

    }
}