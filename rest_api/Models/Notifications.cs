using System;
using System.ComponentModel.DataAnnotations;

namespace rest_api.Models
{
    public class Notifications
    {
        [Key]
        public int id { get; set; }
        [Required]
        public int user_id { get; set; }
        [Required]
        [StringLength(255)]
        public string title { get; set; }
        [Required]
        [StringLength(500)]
        public string detail { get; set; }
        [Required]
        public bool state { get; set; } = false;
        public DateTime created_date { get; set; } = DateTime.Now;
        public DateTime? updated_date { get; set; }

    }
}