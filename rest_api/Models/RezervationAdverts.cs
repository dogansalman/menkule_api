using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rest_api.Models
{
    public class RezervationAdverts
    {
        [Key]
        [Required]
        public int rezervation_id { get; set; }
        [Required]
        [StringLength(255)]
        public string adress { get; set; }
        [Required]
        public float latitude { get; set; }
        [Required]
        public float longitude { get; set; }
        [Required]
        public int cancel_time { get; set; }
        [Required]
        [StringLength(6)]
        public string entry_time { get; set; }
        [Required]
        [StringLength(6)]
        public string exit_time { get; set; }
        [Required]
        public int city_id { get; set; }
        [Required]
        public int town_id { get; set; }
        [Required]
        public int advert_id { get; set; }
        [Required]
        public int user_id { get; set; }
        [Required]
        [StringLength(1200)]
        public string description { get; set; }
        [Required]
        [StringLength(255)]
        public string title { get; set; }
        public DateTime created_date { get; set; } = DateTime.Now;




    }
}