using System;
using System.ComponentModel.DataAnnotations;
using rest_api.Filters;

namespace rest_api.Models
{
    public class RezervationVisitors
    {
        [Key]
        public int id { get; set; }
        [Required]
        public int rezervation_id { get; set; }
        [Required]
        [StringLength(255)]
        public string fullname { get; set; }
        [StringLength(6)]
        [Required]
        [Gender]
        public string gender { get; set; }
        [Required]
        [Range(11,11)]
        public int tc { get; set; }
        public DateTime created_date { get; set; } = DateTime.Now;
    }
}