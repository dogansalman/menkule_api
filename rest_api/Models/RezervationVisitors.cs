using System;
using System.ComponentModel.DataAnnotations;
using rest_api.Filters;

namespace rest_api.Models
{
    public class RezervationVisitors
    {
        [Key]
        public int id { get; set; }
        public int rezervation_id { get; set; }
        [Required]
        [StringLength(255)]
        public string fullname { get; set; }
        [StringLength(6)]
        [Required]
        [Gender]
        public string gender { get; set; }
        [Required]
        [StringLength(11)]
        public string tc { get; set; }
        [EmailAddress]
        public string email { get; set; }
        [PhoneMask("0000000000")]
        [StringLength(11)]
        public string gsm { get; set; }
        public bool manuel_user { get; set; } = false;
        public DateTime created_date { get; set; } = DateTime.Now;
    }
}