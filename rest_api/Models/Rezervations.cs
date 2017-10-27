using System;
using System.ComponentModel.DataAnnotations;
using rest_api.Filters;

namespace rest_api.Models
{
    public class Rezervations
    {
        [Key]
        public int id { get; set; }
        [StringLength(255)]
        public string note { get; set; }
        [Required]
        public int user_id { get; set; }
        [Required]
        public int owner { get; set; }
        [StringLength(255)]
        [Required]
        public string name { get; set; }
        [StringLength(255)]
        [Required]
        public string lastname { get; set; }
        [StringLength(90)]
        [PhoneMask("0000000000")]
        [Required]
        public string gsm { get; set; }
        [Required]
        public int visitor { get; set; }
        [Required]
        public int advert_id { get; set; }
        private int _days;
        public int days
        {
            get { return getDateDiff(checkin, checkout); }
            set { _days = getDateDiff(checkin, checkout); }
        }
        [Required]
        public decimal day_price { get; set; }
        private decimal _total_price;
        public decimal total_price
        {
            get { return (getDateDiff(checkin, checkout) * day_price); }
            set { _total_price = (getDateDiff(checkin, checkout) * day_price); }
        }
        [Required]
        public DateTime checkin { get; set; }
        [Required]
        public DateTime checkout { get; set; }
        [StringLength(255)]
        public string description_state { get; set; }
        public bool state { get; set; } = false;
        public bool is_cancel { get; set; } = false;
        public DateTime created_date { get; set; } = DateTime.Now;
        public DateTime? updated_date { get; set; }
        private int getDateDiff(DateTime checkin, DateTime checkout)
        {
            DateTime StartDate = this.checkin;
            DateTime EndDate = this.checkout;
            return Convert.ToInt32(EndDate.Subtract(StartDate).TotalDays + 1);
        }
    }
}