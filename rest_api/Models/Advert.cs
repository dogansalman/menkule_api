using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using rest_api.ModelViews;

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
        [Required]
        public int zoom { get; set; }
        [Required]
        public double latitude { get; set; }
        [Required]
        public double longitude { get; set; }
        [StringLength(255)]
        public string title { get; set; }
        public DateTime created_date { get; set; } = DateTime.Now;
        public DateTime? updated_date { get; set; }
        [NotMapped]
        public ICollection<AdvertImagesMW> images { get; set; }
        [NotMapped]
        public AdvertProperties properties { get; set; }
        [NotMapped]
        public AdvertPossibilities possibility { get; set; }
        [NotMapped]
        public ICollection<AdvertUnavailableDate> unavaiable_date { get; set; }
        [NotMapped]
        public ICollection<AdvertAvailableDate> avaiable_dates { get; set; }


    }
}