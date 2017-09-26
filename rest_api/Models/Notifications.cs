using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rest_api.Models
{
    public class Notifications
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        public int user_id { get; set; }
        [Required]
        [StringLength(255)]
        public string title { get; set; }
        [Required]
        [StringLength(500)]
        public string detail { get; set; }
        [Range(0, 1)]
        [Required]
        public int state { get; set; } = 0;
        public DateTime created_date { get; set; } = DateTime.Now;
        public DateTime? updated_date { get; set; }

    }
}