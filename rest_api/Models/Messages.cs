using System;
using System.ComponentModel.DataAnnotations;

namespace rest_api.Models
{
    public class Messages
    {
        [Key]
        public int id { get; set; }
        public string messages { get; set; }
        public string last_message { get; set; }
        public DateTime? last_message_on { get; set; }
    }
}