using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using rest_api.Filters;
namespace rest_api.Models
{
    public class Rezervations
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        public int user_id { get; set; }
        [Required]
        [StringLength(255)]
        public string name { get; set; }
        [Required]
        [StringLength(255)]
        public string lastname { get; set; }
        [Required]
        [StringLength(90)]
        [PhoneMask("0000000000")]
        public string gsm { get; set; }
        [Required]
        public int visitor { get; set; }
        [Required]
        public int advert_id { get; set; }
        [Required]
        public int days { get; set; }
        [Required]
        public int day_price { get; set; }
        [Required]
        public int total_price { get; set; }
        [Required]
        public DateTime enter_date { get; set; }
        [Required]
        public DateTime exit_date { get; set; }
        [StringLength(255)]
        public string description_state { get; set; }
        [Range(0,1)]
        public int state { get; set; }
        [Range(0, 1)]
        public int is_cancel { get; set; }
        [StringLength(255)]
        public string note { get; set; }
        public DateTime created_date { get; set; } = DateTime.Now;
        public DateTime? updated_date { get; set; }

    }
}