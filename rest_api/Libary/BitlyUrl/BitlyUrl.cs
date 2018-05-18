using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using Newtonsoft.Json;

namespace rest_api.Libary.BitlyUrl
{
    public static class BitlyUrl
    {
        public class BittyResultObject
        {
            public int status_code { get; set; }
            public string status_txt { get; set; }
            public BittyData data { get; set; }
            
        }

        public class BittyData
        {
            public string hash { get; set; }
            public string global_hash { get; set; }
            public string url { get; set; }
            public string long_url { get; set; }
            public int new_hash { get; set; }
        }
        public static object Short(string longUrl)
        {
            using (var wb = new WebClient())
            {
                var data = new NameValueCollection();
                data["access_token"] = System.Configuration.ConfigurationManager.AppSettings["bitly:appToken"].ToString();
                data["longUrl"] = longUrl;
                string Url = System.Configuration.ConfigurationManager.AppSettings["bitly:apiUrl"].ToString();
                var response = wb.UploadValues(Url, "POST", data);
                BittyResultObject result =  JsonConvert.DeserializeObject<BittyResultObject>(Encoding.UTF8.GetString(response));
                if (result.status_code != 200 || String.IsNullOrEmpty(result.data.url)) return null;
                return result.data.url;
            }
        }
    }
}