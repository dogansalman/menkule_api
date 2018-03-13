using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rest_api.Models
{
    public class RezervationAdverts
    {
        [Key]
        public int id { get; set; }
        [Required]
        [Index("IX_RezervationAdvert", 1, IsUnique = true)]
        public int rezervation_id { get; set; }
        [Required]
        [StringLength(255)]
        public string adress { get; set; }
        [Required]
        public double latitude { get; set; }
        [Required]
        public double longitude { get; set; }
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
        public int advert_type_id { get; set; }
        [Required]
        [Index("IX_RezervationAdvert", 2, IsUnique = true)]
        public int advert_id { get; set; }
        [Required]
        public int user_id { get; set; }
        [Required]
        [StringLength(5000)]
        public string description { get; set; }
        [Required]
        [StringLength(255)]
        public string title { get; set; }
        public DateTime created_date { get; set; } = DateTime.Now;




    }
}