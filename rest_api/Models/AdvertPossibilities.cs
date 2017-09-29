using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rest_api.Models
{
    public class AdvertPossibilities
    {
        [Key]
        public int id { get; set; }
        public int advert_id { get; set; }
        public bool internet { get; set; } = false;
        public bool air { get; set; } = false;
        public bool tv { get; set; } = false;
        public bool jacuzzi { get; set; } = false;
        public bool requiments { get; set; } = false;
        public bool heat { get; set; } = false;
        public bool kitchen { get; set; } = false;
        public bool gym { get; set; } = false;
        public bool elevator { get; set; } = false;
        public bool smoke { get; set; } = false;
        public bool pet { get; set; } = false;
        public DateTime created_date { get; set; } = DateTime.Now;
        public DateTime? updated_date { get; set; }

    }
}