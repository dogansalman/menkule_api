using System;
using System.ComponentModel.DataAnnotations;
using rest_api.Filters;
using System.ComponentModel.DataAnnotations.Schema;

namespace rest_api.Models
{
    public class Users
    {
        
        [Key]
        public int id { get; set; }
        private string _name;
        [Required]
        [StringLength(255)]
        public string name
        {
            get { return _name; }
            set { _name = value.ToLower(); }
        }
        private string _lastname;
        [Required]
        [StringLength(255)]
        public string lastname
        {
            get { return _lastname; }
            set { _lastname = value.ToLower(); }
        }
        private string _email;
        [Required]
        [StringLength(255)]
        [EmailAddress]
        [Index("IX_UserEmail", 1, IsUnique = true)]
        public string email
        {
            get { return _email; }
            set { _email = value.ToLower(); }
        }

        [Required]
        [StringLength(255)]
        public string password { get; set; }
        [Required]
        [PhoneMask("0000000000")]
        [StringLength(11)]
        [Index("IX_UserGsm", 2, IsUnique = true)]
        public string gsm { get; set; }
        public int? image_id { get; set; }
        public bool email_state { get; set; } = false;
        public bool gsm_state { get; set; } = false;
        [StringLength(255)]
        public string email_activation_code { get; set; }
        [StringLength(255)]
        public string gsm_activation_code { get; set; }
        [StringLength(5)]
        [Gender]
        [Required]
        public string gender { get; set; }
        [StringLength(90)]
        public string source { get; set; }
        [StringLength(255)]
        public string facebook_id { get; set; }
        public DateTime? gsm_last_update { get; set; }
        public bool ownershiping { get; set; }
        public bool state { get; set; }
        [StringLength(255)]
        public string description { get; set; }
        public DateTime? forgot_last_date { get; set; }
        public DateTime created_date { get; set; } = DateTime.Now;
        public DateTime? updated_date { get; set; }

    }
}