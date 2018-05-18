using rest_api.Filters;
using System;
using System.ComponentModel.DataAnnotations;

namespace rest_api.ModelViews
{
    public class _RezervationShareInfo
    {
        [PhoneMask("0000000000")]
        public string gsm { get; set; }
        [EmailAddress]
        public string email { get; set; }
        [Required]
        public DateTime checkin { get; set; }
        [Required]
        public DateTime checkout { get; set; }
        [Required]
        public int advert_id { get; set; }
        [Required]
        [StringLength(155)]
        public string fullname { get; set; }
    }
}