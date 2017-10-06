using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace rest_api.Libary.Responser
{
    public static class Responser
    {
        public static void Response(HttpStatusCode status, string message)
        {
            throw new HttpResponseException(new HttpResponseMessage(status) { Content = new StringContent("{\"Message\":\"" + message + "\"}", System.Text.Encoding.UTF8, "application/json") });
        }
    }
}