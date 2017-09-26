using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rest_api.Models
{
    public class AdvertProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        public int advert_id { get; set; }
        [Required]
        public int visitor { get; set; }
        [Required]
        public int bathroom { get; set; }
        [Required]
        public int m2 { get; set; }
        [Required]
        public int room { get; set; }
        [Required]
        public int beds { get; set; }
        [Required]
        public int bedroom { get; set; }
        [Required]
        public int build_age { get; set; }
        [Required]
        public int floor { get; set; }
        [Required]
        public int hall { get; set; }
        public DateTime created_date { get; set; } = DateTime.Now;
        public DateTime? updated_date { get; set; }
    }
}