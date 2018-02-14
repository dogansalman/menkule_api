using System;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace rest_api.Libary.Exceptions.ExceptionThrow
{
    public static class ExceptionThrow
    {
        public static void Throw(Exception e = null, HttpStatusCode code = HttpStatusCode.NotImplemented, string message = null)
        {
            if(e != null)
            {
                while (e.InnerException != null) e = e.InnerException;
                if (e.GetType() == typeof(SqlException))
                {
                    e = (SqlException)e as SqlException;
                    SqlException x = (SqlException)e as SqlException;
                    if (x.Number == 2627) throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotImplemented) { Content = new StringContent("{\"Message\":\"Handle unique constraint violation.\"}", System.Text.Encoding.UTF8, "application/json") });
                }

                throw new HttpResponseException(new HttpResponseMessage(code) { Content = new StringContent("{\"Message\":\"" + e.Message.ToString() + "\"}", System.Text.Encoding.UTF8, "application/json") });
            }
           
           
        }

        public static void Throw(string message, HttpStatusCode code = HttpStatusCode.NotImplemented)
        {
            throw new HttpResponseException(new HttpResponseMessage(code) { Content = new StringContent("{\"Message\":\"" + message + "\"}", System.Text.Encoding.UTF8, "application/json") });
        }
    }
}