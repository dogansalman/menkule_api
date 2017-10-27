using rest_api.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace rest_api.ModelViews
{
    public class _Rezervation
    {
        [Required]
        public ICollection<RezervationVisitors> visitors { get; set; }
        [Required]
        public DateTime checkin { get; set; }
        [Required]
        public DateTime checkout { get; set; }
        [Required]
        public int advert_id { get; set; }
        [StringLength(255)]
        public string note { get; set; }
    }
}