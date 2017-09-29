using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rest_api.Models
{
    public class AdvertUnavailableDate
    {
        [Key]
        public int id { get; set; }
        [Required]
        public int day { get; set; }
        [Required]
        public int month { get; set; }
        [Required]
        public int year { get; set; }
        [Required]
        public DateTime fulldate { get; set; }
        public int rezervation_id { get; set; }
        public int advert_id { get; set; }
        public DateTime created_date { get; set; } = DateTime.Now;

    }
}