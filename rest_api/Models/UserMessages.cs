using System;
using System.ComponentModel.DataAnnotations;

namespace rest_api.Models
{
    public class UserMessages
    {
        [Key]
        public int id { get; set; }
        [Required]
        public int user_id { get; set; }
        [Required]
        public int message_id { get; set; }
        public bool is_owner { get; set; } = false;
        public DateTime? last_view { get; set; }
        public bool is_deleted { get; set; } = false;
    }
}