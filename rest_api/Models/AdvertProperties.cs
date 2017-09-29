using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rest_api.Models
{
    public class AdvertProperties
    {
        [Key]
        public int id { get; set; }
        public int advert_id { get; set; }
        public int visitor { get; set; }
        public int bathroom { get; set; }
        public int m2 { get; set; }
        public int room { get; set; }
        public int beds { get; set; }
        public int bedroom { get; set; }
        public int build_age { get; set; }
        public int floor { get; set; }
        public int hall { get; set; }
        public DateTime created_date { get; set; } = DateTime.Now;
        public DateTime? updated_date { get; set; }
    }
}