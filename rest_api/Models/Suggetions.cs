using System;
using System.ComponentModel.DataAnnotations;

namespace rest_api.Models
{
    public class Suggetions
    {
        [Key]
        [Required]
        [StringLength(255)]
        public string email { get; set; }
        public DateTime created_date { get; set; } = DateTime.Now;
    }
}