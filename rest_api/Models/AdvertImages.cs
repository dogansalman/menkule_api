using System;
using System.ComponentModel.DataAnnotations;

namespace rest_api.Models
{
    public class AdvertImages
    {
        [Key]
        public int id { get; set; }
        public int image_id { get; set; }
        public int advert_id { get; set; }
        public bool is_default { get; set; } = false;
        public DateTime created_date { get; set; } = DateTime.Now;
    }
}