using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rest_api.Models
{
    public class AdvertPossibilities
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        public int advert_id { get; set; }
        public int internet { get; set; } = 0;
        public int air { get; set; } = 0;
        public int tv { get; set; } = 0;
        public int jacuzzi { get; set; } = 0;
        public int requiments { get; set; } = 0;
        public int heat { get; set; } = 0;
        public int kitchen { get; set; } = 0;
        public int gym { get; set; } = 0;
        public int elevator { get; set; } = 0;
        public int smoke { get; set; } = 0;
        public int pet { get; set; } = 0;
        public DateTime created_date { get; set; } = DateTime.Now;
        public DateTime? updated_date { get; set; }

    }
}