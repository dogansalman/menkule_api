using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rest_api.Models
{
    public class AdvertAvailableDate
    {
        [Key]
        public int id { get; set; }
        public int day { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public DateTime fulldate { get; set; }
        [StringLength(25)]
        public string uniq { get; set; }
        public int advert_id { get; set; }
        public DateTime created_date { get; set; } = DateTime.Now;
        [NotMapped]
        public DateTime from { get; set; }
        [NotMapped]
        public int from_day { get; set; }
        [NotMapped]
        public DateTime from_fulldate { get; set; }
        [NotMapped]
        public int from_month { get; set; }
        [NotMapped]
        public int from_year { get; set; }
        [NotMapped]
        public DateTime to { get; set; }
        [NotMapped]
        public int to_day { get; set; }
        [NotMapped]
        public DateTime to_fulldate { get; set; }
        [NotMapped]
        public int to_month { get; set; }
        [NotMapped]
        public int to_year { get; set; }
     
        public void setDate()
        {

        }
    }
}