using System;
using System.Net;
using System.Text;
using rest_api.Models;

namespace rest_api.Libary.NetGsm
{
    public static class NetGsm
    {
        static string XMLPOST(string PostAddress, string xmlData)
        {
            try
            {
                WebClient wUpload = new WebClient();
                HttpWebRequest request = WebRequest.Create(PostAddress) as HttpWebRequest;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                Byte[] bPostArray = Encoding.UTF8.GetBytes(xmlData);
                Byte[] bResponse = wUpload.UploadData(PostAddress, "POST", bPostArray);
                Char[] sReturnChars = Encoding.UTF8.GetChars(bResponse);
                string sWebPage = new string(sReturnChars);
                return sWebPage;
            }
            catch
            {
                return "-1";
            }
        }

        public static bool Send(string gsm, string message)
        {
            // Uniq sms send
            string uniq = Users.generatePassword(3, 3);
            message += " ref:" + uniq;
    
            if (gsm == "") return false;
            gsm = gsm.Replace("-", "");
            gsm = gsm.Replace(" ", "");
            string ss = "";
            ss += "<?xml version='1.0' encoding='UTF-8'?>";
            ss += "<mainbody>";
            ss += "<header>";
            ss += "<company>*****************</company>";
            ss += "<usercode>*****************</usercode>";
            ss += "<password>*****************</password>";
            ss += "<startdate></startdate>";
            ss += "<stopdate></stopdate>";
            ss += "<type>1:n</type>";
            ss += "<msgheader>*****************</msgheader>";
            ss += "</header>";
            ss += "<body>";
            ss += "<msg><![CDATA[" + message + "]]></msg>";
            ss += "<no>90" + gsm + "</no>";
            ss += "</body>";
            ss += "</mainbody>";
            XMLPOST("http://api.netgsm.com.tr/xmlbulkhttppost.asp", ss);
            return true;
        }

    }
}
