using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rest_api.Models
{
    public class AdvertTypes
    {
        [Key]
        public int id { get; set; }
        [Required]
        [StringLength(255)]
        public string name { get; set; }
        public DateTime created_date { get; set; } = DateTime.Now;
        public DateTime? updated_date { get; set; }
    }
}