﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using rest_api.ModelViews;

namespace rest_api.Models
{
    public class Advert
    {
        [Key]
        public int id { get; set; }
        [Required]
        public int advert_type_id { get; set; }
        [MaxLength(255)]
        public string adress { get; set; }
        [Required]
        public int city_id { get; set; }
        [Required]
        public int town_id { get; set; }
        public int user_id { get; set; }
        [Required]
        public string entry_time { get; set; }
        [Required]
        public string exit_time { get; set; }
        public bool state { get; set; } = false;
        public bool is_cancel { get; set; } = false;
        [StringLength(600)]
        public string cancel_description { get; set; }
        public int views { get; set; }
        public int score { get; set; }
        [Required]
        public decimal price { get; set; }
        [Required]
        public int min_layover { get; set; }
        [Required]
        public int cancel_time { get; set; }
        [StringLength(10000)]
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
        [Required]
        public ICollection<_AdvertImages> images { get; set; }
        [NotMapped]
        [Required]
        public AdvertProperties properties { get; set; }
        [NotMapped]
        [Required]
        public AdvertPossibilities possibility { get; set; }
        [NotMapped]
        public ICollection<AdvertUnavailableDate> unavaiable_date { get; set; }
        [NotMapped]
        public ICollection<AdvertAvailableDate> available_date { get; set; }
      

    }
    
}