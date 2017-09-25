using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace rest_api.Models
{
    public class Advert
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        public int advert_type_id { get; set; }
        [MaxLength(255)]
        public string adress { get; set; }
        [Required]
        public int city_id { get; set; }
        [Required]
        public int town_id { get; set; }
        [Required]
        public int user_id { get; set; }
        [Required]
        public string entry_time { get; set; }
        [Required]
        public string exit_time { get; set; }
        [Required]
        public Boolean state { get; set; }
        public int views { get; set; }
        public int score { get; set; }
        [Required]
        public decimal price { get; set; }
        [Required]
        public int min_layover { get; set; }
        [Required]
        public int cancel_time { get; set; }
        public string description { get; set; }
        public DateTime date { get; set; } = DateTime.Now;
        [Required]
        public int zoom { get; set; }
        [Required]
        public double latitude { get; set; }
        [Required]
        public double longitude { get; set; }
        [StringLength(255)]
        public string title { get; set; }

    }
}