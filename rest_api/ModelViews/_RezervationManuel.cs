using rest_api.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace rest_api.ModelViews
{
    public class _RezervationManuel
    {
        [Required]
        public ICollection<RezervationVisitors> visitors { get; set; }
        [Required]
        public DateTime checkin { get; set; }
        [Required]
        public DateTime checkout { get; set; }
        [Required]
        public int advert_id { get; set; }
        [Required]
        [StringLength(255)]
        public string name { get; set; }
        [Required]
        [StringLength(255)]
        public string lastname { get; set; }
        [Required]
        [Phone]
        public string gsm { get; set; }
        [Required]
        [EmailAddress]
        public string email { get; set; }

    }
}