using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using rest_api.Filters;
namespace rest_api.Models
{
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        [StringLength(255)]
        public string name { get; set; }
        [Required]
        [StringLength(255)]
        public string lastname { get; set; }
        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string email { get; set; }
        [Required]
        [StringLength(255)]
        public string password { get; set; }
        [Required]
        [PhoneMask("0000000000")]
        public string gsm { get; set; }
        [StringLength(255)]
        public string photo { get; set; }
        [Range(0,1)]
        public int email_state { get; set; }
        [Range(0, 1)]
        public int gsm_state { get; set; }
        [StringLength(255)]
        public string email_activation_code { get; set; }
        [StringLength(255)]
        public string gsm_activation_code { get; set; }
        [StringLength(5)]
        [Gender]
        public string gender { get; set; }
        [StringLength(90)]
        public string source { get; set; }
        [StringLength(255)]
        public string facebook_id { get; set; }
        public DateTime gsm_last_update { get; set; }
        [Range(0, 1)]
        public int ownershiping { get; set; }
        [StringLength(255)]
        public string description { get; set; }
        public DateTime forgot_last_date { get; set; }
        public DateTime created_date { get; set; } = DateTime.Now;
        public DateTime? updated_date { get; set; }

    }
}