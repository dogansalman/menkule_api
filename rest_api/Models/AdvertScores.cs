using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rest_api.Models
{
    public class AdvertScores
    {
        [Key]
        public int id { get; set; }
        [Required]
        [Range(1,5)]
        public int score { get; set; }
        [Index("IX_AdvertScore", 1, IsUnique = true)]
        public int user_id { get; set; }
        [Required]
        [Index("IX_AdvertScore", 2, IsUnique = true)]
        public int advert_id { get; set; }
        public DateTime created_date { get; set; } = DateTime.Now;
        public DateTime? updated_date { get; set; }
    }
}