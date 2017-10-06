using System;
using System.ComponentModel.DataAnnotations;

namespace rest_api.Models
{
    public class AdvertComments
    {
        [Key]
        public int id { get; set; }
        [Required]
        public int advert_id { get; set; }
        [Required]
        [StringLength(500)]
        public string comment { get; set; }
        public int user_id { get; set; }
        public bool state { get; set; } = false;
        public DateTime created_date { get; set; } = DateTime.Now;
        public DateTime? updated_date { get; set; }

    }
}