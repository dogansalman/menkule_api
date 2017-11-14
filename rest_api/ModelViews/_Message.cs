using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace rest_api.ModelViews
{
    public class _MessageDetail
    {
     
        public string fullname { get; set; }
        public string message { get; set; }
        public DateTime date { get; set; }
    }

    public class _MessageDetailList
    {
        public ICollection<_MessageDetail> messages { get; set; }
        public int user_id { get; set; }
    }
    public class _Message
    {
        [Required]
        [StringLength(300)]
        public string message { get; set; }
        [Required]
        public int user_id { get; set; }
    }
}