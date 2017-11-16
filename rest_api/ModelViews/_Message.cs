using Newtonsoft.Json;
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
        public int id { get; set; }
        private object _message;
        public object message
        {
            get
            {
                return JsonConvert.DeserializeObject<ICollection<_MessageDetail>>(_message.ToString());
            }
            set { _message = value; }

        }
        public DateTime? last_view { get; set; }
        public _MessageUser user { get; set; }

    }
    public class _Message
    {
        [Required]
        [StringLength(300)]
        public string message { get; set; }
        [Required]
        public int user_id { get; set; }
    }

    public class _ReplyMessage
    {
        [Required]
        [StringLength(300)]
        public string message { get; set; }
    }

    public class _Messages
    {
        public int id { get; set; }
        private object _message;

        public object message
        {
            get
            {
                return JsonConvert.DeserializeObject<_MessageDetail>(_message.ToString());
            }
            set { _message = value; }
        }
        public DateTime? last_view { get; set; }
        public _MessageUser user { get; set; }
    }

    public class _MessageUser
    {
        public string fullname { get; set; }
        public string photo { get; set; }
    }
}