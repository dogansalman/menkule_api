using System;
using System.Collections.Generic;
using System.Web;
using RestSharp;
using RestSharp.Authenticators;
using System.IO;


namespace rest_api.Libary.Mailgun
{
    public static class Mailgun
    {
        private static string baseUrl = "https://api.mailgun.net/v3";
        private static string apiKey = "key-6ff5be8305d311728de6515662eba7c5";
        private static string domain = "notify.menkule.com.tr";
        private static string senderAdress = "Menkule <notify@menkule.com.tr>";


        public static void Send(string templateName, Dictionary<string, object> recipientVariables, string toAdress, string subject)
        {
            
            
            RestClient client = new RestClient();
            client.BaseUrl = new Uri(baseUrl);
            client.Authenticator =
                new HttpBasicAuthenticator("api", apiKey);
            RestRequest request = new RestRequest();
            request.AddParameter("domain", domain, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", senderAdress);
            request.AddParameter("to", toAdress);
            request.AddParameter("subject", subject);
            request.AddParameter("html", RenderTemplate(templateName));
            request.AddParameter("recipient-variables", "{\"" + toAdress + "\": " + JsonHelper.JsonHelper.DictionaryToJson(recipientVariables) + "}");
            request.Method = Method.POST;
            client.Execute(request);
        }

        private static string RenderTemplate(string templateName)
        {
            string templatesrc = HttpContext.Current.Server.MapPath("~/mail_templates/" + templateName + ".html");
            string templateString = "";
            using (StreamReader reader = new StreamReader(templatesrc, true))
            {
                templateString = reader.ReadToEnd();
            }
            return templateString;
        }
    }
}