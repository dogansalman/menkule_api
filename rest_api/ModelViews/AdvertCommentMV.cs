using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rest_api.ModelViews
{
    public class AdvertCommentMV
    {
        public int id { get; set; }
        public string comment { get; set; }
        public int state { get; set; }
        public string fullname { get; set; }
        public int user_id { get; set; }
        public string photo { get; set; }
        public DateTime created_date { get; set; }
    }
}