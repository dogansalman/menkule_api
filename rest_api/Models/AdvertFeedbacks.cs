using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace rest_api.Models
{
    public class AdvertFeedbacks
    {
        [Key]
        public int id { get; set; }
        [Required]
        public int advert_id { get; set; }
        [Required]
        [StringLength(300, ErrorMessage = "Açıklama en fazla 300 karakter olabilir.")]
        public string description { get; set; }
        public int user_id { get; set; }
        [Required]
        [StringLength(255)]
        public string fullname { get; set; }
        [Required]
        [StringLength(255)]
        [EmailAddress(ErrorMessage = "E-posta adresi hatalı")]
        public string email { get; set; }
        public DateTime created_date { get; set; } = DateTime.Now;
        public DateTime? updated_date { get; set; }

    }
}