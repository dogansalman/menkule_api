using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rest_api.Models
{
    public class AdvertImages
    {
        [Key]
        public int id { get; set; }
        [Index("IX_AdvertImage", 1, IsUnique = true)]
        public int image_id { get; set; }
        [Index("IX_AdvertImage", 2, IsUnique = true)]
        public int advert_id { get; set; }
        [NotMapped]
        public bool is_new { get; set; } = false;
        [NotMapped]
        public bool deleted { get; set; } = false;
        public bool is_default { get; set; } = false;
        public DateTime created_date { get; set; } = DateTime.Now;
    }
}