using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using rest_api.Filters;
using System;

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
        [Required]
        [StringLength(255, ErrorMessage = "IP Adresi En fazla 255 karakter olabilir.")]
        [IpAdress(ErrorMessage = "Ip adresi hatalı")]
        public string ip { get; set; }
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