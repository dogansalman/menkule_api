using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rest_api.Models
{
    public class AdvertImages
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        public int image_id { get; set; }
        [Required]
        public int advert_id { get; set; }
        public DateTime created_date { get; set; } = DateTime.Now;

    }
}