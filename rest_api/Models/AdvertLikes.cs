using System;
using System.ComponentModel.DataAnnotations;

namespace rest_api.Models
{
    public class AdvertLikes
    {
        [Key]
        public int id { get; set; }
        [Required]
        public int user_id { get; set; }
        public DateTime created_date { get; set; } = DateTime.Now;
    }
}